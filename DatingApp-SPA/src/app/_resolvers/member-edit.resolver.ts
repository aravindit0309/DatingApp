import { Injectable } from '@angular/core';
import { User } from '../_models/User';
import { Resolve, Router, ActivatedRoute, ActivatedRouteSnapshot } from '@angular/router';
import { UserService } from '../_services/user.service';
import { AlertifyService } from '../_services/alertify.service';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { AuthService } from '../_services/Auth.service';

@Injectable()

export class MemberEditResolver implements Resolve<User> {

    constructor(private userService: UserService, private router: Router, private alertify: AlertifyService,
                private authService: AuthService ){}

    resolve(route: ActivatedRouteSnapshot): Observable<User>{
        console.log(this.authService.decodedToken);
        return this.userService.getUser(this.authService.decodedToken.nameid).pipe(
            catchError( err => {
                this.alertify.error('Problem retrieving your data');
                this.router.navigate(['/members']);
                return of(null);
            })
        )

    }
}