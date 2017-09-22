import {DataSource} from '@angular/cdk/collections';
import {Observable} from 'rxjs/Observable';
import { Subject }    from 'rxjs/Subject';
import { Http } from '@angular/http';

export class DriveDataSource extends DataSource<any>{
	private _url: string;
	private _Message: Subject<any> = new Subject<any>();
	public Message: Observable<any> = this._Message.asObservable();
	private _Path: Array<string> = new Array<string>();
	
	constructor(url: string, private http: Http) {
		super();
		this._url = url;
	}

	public get Path():string {
		return this._Path.join("/");
	}

	public get PathList(): Array<string> {
		return this._Path;
	}

	public NavigateToChild(childName: string) {
		this._Message.next(null);
		this._Path.push(childName);
		this._FetchPathItems();
	}

	public NavigateToParent() {
		this._Message.next(null);
		this._Path.pop();
		this._FetchPathItems();
	}

	public NavigateToRoot() {
		this._Message.next(null);
		this._Path = new Array<string>();
		this._FetchPathItems();
	}

	private _FetchPathItems() {
		var listUrl = this._url + "/GetDriveItems/"
		this.http.post(listUrl, {
			"path": this.Path
		}).subscribe(response => {
			var results = response.json();
			console.log(results);
			this._Message.next(results);
		}, error => {

		});
	}


	connect(): Observable<any[]> {
		this._FetchPathItems();
		return this.Message;
	}

	disconnect() { }
}