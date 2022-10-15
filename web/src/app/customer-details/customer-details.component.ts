import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ActivatedRoute } from '@angular/router';
import { Observable } from 'rxjs';
import { Customer } from '../customer';
import { CustomerResourceService } from '../customer-resource-service';

@Component({
  selector: 'app-customer-details',
  templateUrl: './customer-details.component.html',
  styleUrls: ['./customer-details.component.css']
})
export class CustomerDetailsComponent implements OnInit {

  customer: Observable<Customer> | undefined;

  constructor(private route: ActivatedRoute, private customerService: CustomerResourceService) { }

  ngOnInit(): void {
    const routeParams = this.route.snapshot.paramMap;
    const customerIdFromRoute = Number(routeParams.get('customerId'));

    this.customer = this.customerService.getResource(customerIdFromRoute);
  }

}
