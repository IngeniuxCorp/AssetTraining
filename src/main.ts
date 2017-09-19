import { platformBrowserDynamic } from '@angular/platform-browser-dynamic';
import { CustomTabAppNgFactory } from '../aot/src/app/customTabApp.module.ngfactory';

console.log('Running AOT compiled');
platformBrowserDynamic().bootstrapModuleFactory(CustomTabAppNgFactory);
