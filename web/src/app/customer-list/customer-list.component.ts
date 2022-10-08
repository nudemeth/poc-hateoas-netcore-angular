import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { Customer } from '../customer';

@Component({
  selector: 'app-customer-list',
  templateUrl: './customer-list.component.html',
  styleUrls: ['./customer-list.component.css']
})
export class CustomerListComponent implements OnInit {

  customers: Observable<Customer[]> | undefined;

  constructor(private http: HttpClient) {
  }

  ngOnInit(): void {
    this.customers = this.http.get<{items: Customer[]}>("https://localhost:7021/Customer").pipe(map(res => res.items));
  }

}
