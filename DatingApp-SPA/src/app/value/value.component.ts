import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';


@Component({
  selector: 'app-value',
  templateUrl: './value.component.html',
  styleUrls: ['./value.component.css']
})
export class ValueComponent implements OnInit {

values: any;
  constructor(private htttp: HttpClient) { }

  ngOnInit() {
    this.getValues();
  }
getValues() {
  this.htttp.get('http://localhost:5000/api/values').subscribe(Response => {
  this.values = Response;
  }, error => {
  console.log(error);
});
}

}

