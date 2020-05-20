import { Component, OnInit } from '@angular/core';
import { Message } from '../_models/Message';
import { Pagination, PaginatedResult } from '../_models/Pagination';
import { UserService } from '../_services/user.service';
import { AuthService } from '../_services/Auth.service';
import { ActivatedRoute } from '@angular/router';
import { AlertifyService } from '../_services/alertify.service';
import { tap } from 'rxjs/operators';


@Component({
  selector: 'app-messages',
  templateUrl: './messages.component.html',
  styleUrls: ['./messages.component.css']
})
export class MessagesComponent implements OnInit {
  messages: Message[];
  pagination: Pagination;
  messageContainer = 'Unread';

  constructor(private userService: UserService, private authService: AuthService, private route: ActivatedRoute,
              private alertify: AlertifyService) { }

  ngOnInit() {
    this.route.data.subscribe(data =>{
      this.messages = data['messages'].result;
      this.pagination = data['messages'].pagination;
    });
  }

  loadMessages(){
    const userId = +this.authService.decodedToken.nameid;
    this.userService.getMessages(this.authService.decodedToken.nameid, this.pagination.currentPage,
       this.pagination.itemsPerPage, this.messageContainer)
       .pipe(
         tap( messages => {
           // tslint:disable-next-line: prefer-for-of
           for (let i = 0; i < messages.result.length; i++){
              if (messages.result[i].isRead === false && messages.result[i].recipientId === userId){
                this.userService.markAsRead(userId, messages.result[i].id);
              }
           }
         }

         )
       )
       .subscribe( (res: PaginatedResult<Message[]>) => {
              this.messages = res.result;
              this.pagination = res.pagination;
            }, err =>{
              this.alertify.error( 'inside load messages: ' + err);
            });
  }

  deleteMessage(id: number){
    this.alertify.confirm('Are you sure you want to delete the message', () =>{
      this.userService.deleteMessage(id, this.authService.decodedToken.nameid).subscribe(
        () => {
          this.messages.splice(this.messages.findIndex(x => x.id === id), 1);
          this.alertify.success('Message has been deleted');
        }, err => {
          this.alertify.error('Failed to delete the message');
        });
    });
  }

  pageChanged(event: any): void {
    this.pagination.currentPage = event.page;
    this.loadMessages();
  }

}
