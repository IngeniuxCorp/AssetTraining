"use strict";
var __extends = (this && this.__extends) || (function () {
    var extendStatics = Object.setPrototypeOf ||
        ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
        function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
Object.defineProperty(exports, "__esModule", { value: true });
var collections_1 = require("@angular/cdk/collections");
var Subject_1 = require("rxjs/Subject");
var DriveDataSource = (function (_super) {
    __extends(DriveDataSource, _super);
    function DriveDataSource(url, http) {
        var _this = _super.call(this) || this;
        _this.http = http;
        _this._Message = new Subject_1.Subject();
        _this.Message = _this._Message.asObservable();
        _this._Path = new Array();
        _this._url = url;
        return _this;
    }
    Object.defineProperty(DriveDataSource.prototype, "Path", {
        get: function () {
            return this._Path.join("/");
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(DriveDataSource.prototype, "PathList", {
        get: function () {
            return this._Path;
        },
        enumerable: true,
        configurable: true
    });
    DriveDataSource.prototype.NavigateToChild = function (childName) {
        this._Message.next(null);
        this._Path.push(childName);
        this._FetchPathItems();
    };
    DriveDataSource.prototype.NavigateToParent = function () {
        this._Message.next(null);
        this._Path.pop();
        this._FetchPathItems();
    };
    DriveDataSource.prototype.NavigateToRoot = function () {
        this._Message.next(null);
        this._Path = new Array();
        this._FetchPathItems();
    };
    DriveDataSource.prototype._FetchPathItems = function () {
        var _this = this;
        var listUrl = this._url + "/GetDriveItems/";
        this.http.post(listUrl, {
            "path": this.Path
        }).subscribe(function (response) {
            var results = response.json();
            console.log(results);
            _this._Message.next(results);
        }, function (error) {
        });
    };
    DriveDataSource.prototype.connect = function () {
        this._FetchPathItems();
        return this.Message;
    };
    DriveDataSource.prototype.disconnect = function () { };
    return DriveDataSource;
}(collections_1.DataSource));
exports.DriveDataSource = DriveDataSource;
//# sourceMappingURL=driveDataSource.js.map