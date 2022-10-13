import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule } from '@angular/common/http';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { CustomerListComponent } from './customer-list/customer-list.component';
import { CustomerDetailsComponent } from './customer-details/customer-details.component';

import { NgxHateoasClientModule, NgxHateoasClientConfigurationService  } from '@lagoshny/ngx-hateoas-client';
import { Customer } from './customer';

@NgModule({
  declarations: [
    AppComponent,
    CustomerListComponent,
    CustomerDetailsComponent
  ],
  imports: [
    BrowserModule,
    HttpClientModule,
    AppRoutingModule,
    NgxHateoasClientModule.forRoot()
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule {
  constructor(hateoasConfig: NgxHateoasClientConfigurationService) {
    hateoasConfig.configure({
      http: {
        rootUrl: 'https://localhost:7021'
      },
      useTypes: {
        resources: [Customer]
      }
    });
  }
 }
