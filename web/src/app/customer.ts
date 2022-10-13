import { Resource, HateoasResource } from '@lagoshny/ngx-hateoas-client';

@HateoasResource('customers')
export class Customer extends Resource {
    public id: number | undefined;
    public name: string | undefined;
    public createdAt: Date | undefined;

    constructor() {
        super();
    }
}
