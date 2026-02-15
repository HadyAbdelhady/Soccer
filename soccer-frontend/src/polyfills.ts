import { platformBrowserDynamic } from '@angular/platform-browser-dynamic';

platformBrowserDynamic()
  .bootstrapModule(require('./app/app.component').AppComponent)
  .catch(err => console.log(err));
