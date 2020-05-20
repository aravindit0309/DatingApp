import { Component, OnInit } from '@angular/core';
import { User } from '../_models/User';
import { Pagination, PaginatedResult } from '../_models/Pagination';
import { AuthService } from '../_services/Auth.service';
import { UserService } from '../_services/user.service';
import { ActivatedRoute } from '@angular/router';
import { AlertifyService } from '../_services/alertify.service';

@Component({
  selector: 'app-lists',
  templateUrl: './lists.component.html',
  styleUrls: ['./lists.component.css']
})
export class ListsComponent implements OnInit {
  users: User[] ;
  pagination: Pagination;
  likesParam: string;

  constructor(private authService: AuthService, private userService: UserService,
              private route: ActivatedRoute, private alertify: AlertifyService) {
      }

  ngOnInit() {
    //console.log('Inside Lists componenet');
    this.route.data.subscribe(data => {
      // console.log(data);
      this.users = data['user'].result;
      console.log(this.users);
      this.pagination = data['user'].pagination;
    });
    this.likesParam = 'Likers';
    // this.loadUsers();
  }

  loadUsers(){
    this.userService.getusers(this.pagination.currentPage, this.pagination.itemsPerPage,
      null, this.likesParam).subscribe(
      (res: PaginatedResult<User[]>) => {
      this.users =  res.result;
      this.pagination = res.pagination;
    }, error => {
      this.alertify.error(error);
    });
  }

  pageChanged(event: any): void {
    this.pagination.currentPage = event.page;
    // console.log(this.pagination.currentPage);
    this.loadUsers();
  }

}
