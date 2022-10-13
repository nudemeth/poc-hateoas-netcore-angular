import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { Customer } from '../customer';
import { CustomerResourceService } from '../customer-resource-service';
import { SortOrder } from '@lagoshny/ngx-hateoas-client';

@Component({
  selector: 'app-customer-list',
  templateUrl: './customer-list.component.html',
  styleUrls: ['./customer-list.component.css']
})
export class CustomerListComponent implements OnInit {

  customers: Observable<Customer[]> | undefined;

  constructor(private customerService: CustomerResourceService) {
  }

  ngOnInit(): void {
    const queryParams = {
      sort: {
        id: 'ASC' as SortOrder
      }
    };
    this.customers = this.customerService.getCollection(queryParams).pipe(map(res => res.resources));
  }

}
