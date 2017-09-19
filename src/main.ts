import { platformBrowserDynamic } from '@angular/platform-browser-dynamic';
import { CustomTabAppNgFactory } from '../aot/src/app/customTabApp.module.ngfactory';
import {enableProdMode} from '@angular/core';
import "jquery";
import "signalr";


enableProdMode();
platformBrowserDynamic().bootstrapModuleFactory(CustomTabAppNgFactory);
