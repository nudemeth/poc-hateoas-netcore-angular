import { Injectable } from '@angular/core';
import { Resource, HateoasResource, HateoasResourceOperation } from '@lagoshny/ngx-hateoas-client';
import { Customer } from './customer';

@Injectable({providedIn: 'root'})
export class CustomerResourceService extends HateoasResourceOperation<Customer> {

  constructor() {
    super(Customer);
  }

}