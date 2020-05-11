import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { User } from '../_models/User';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class UserService {
baseUrl: string;

constructor(private http: HttpClient) {
  this.baseUrl = environment.apiUrl;
}

getusers(): Observable<User[]>{
  return this.http.get<User[]>(this.baseUrl + 'users');
}

getUser(id): Observable<User>{  
  return this.http.get<User>(this.baseUrl + 'users/' + id);
}

updateUser(id: number, user: User){
  return this.http.put(this.baseUrl + 'users/' + id, user);
}

setMain(userId: number, id: number){
return this.http.post(this.baseUrl + 'users/' + userId + '/photos/' + id + '/setMain', {});
}

deletePhoto(userId: number, id: number){
  // console.log(userId);
  return this.http.delete(this.baseUrl + 'users/' + userId + '/photos/' + id );
  }

}
