import { Injectable } from '@angular/core';
import { Subject }    from 'rxjs/Subject';
import { Observable }    from 'rxjs/Observable';

@Injectable()
export class SignalRService {
	private _Messenger: any;
	private _Message: Subject<any> = new Subject<any>();
	public Message: Observable<any> = this._Message.asObservable();

	constructor() {
		this._Messenger = window["jQuery"].connection.messenger;
		this._Messenger.client.serverRestMessage = this._MessageHandler.bind(this);
		window["jQuery"].connection.hub.start();
	}

	private _MessageHandler(data: string) {
		let response = JSON.parse(data);
		this._Message.next(response);
	}

}