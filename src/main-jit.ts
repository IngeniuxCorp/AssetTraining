import { platformBrowserDynamic } from '@angular/platform-browser-dynamic';
import { CustomTabApp } from './app/customTabApp.module';
import "jquery";
import "signalr";

platformBrowserDynamic().bootstrapModule(CustomTabApp);
