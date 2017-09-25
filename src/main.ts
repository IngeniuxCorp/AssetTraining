import { platformBrowserDynamic } from '@angular/platform-browser-dynamic';
import { CustomTabAppNgFactory } from '../aot/src/app/customTabApp.module.ngfactory';
import {enableProdMode} from '@angular/core';

enableProdMode();
platformBrowserDynamic().bootstrapModuleFactory(CustomTabAppNgFactory);
