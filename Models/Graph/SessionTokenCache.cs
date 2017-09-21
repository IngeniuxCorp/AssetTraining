/* 
*  Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license. 
*  See LICENSE in the source repository root for complete license information. 
*/

using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;

namespace v10CustomTabQuickStart.Models.Graph
{

    // Store the user's token information.
    public class SessionTokenCache
    {
        private static ReaderWriterLockSlim SessionLock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
        string UserId = string.Empty;
        string CacheId = string.Empty;
		HttpContextBase httpContext = null;

        public SessionTokenCache(string userId, HttpContextBase httpcontext)
        {
            // not object, we want the SUB
            UserId = userId;
            CacheId = UserId + "_TokenCache";
            httpContext = httpcontext;
        }

        public void SaveUserStateValue(string state)
        {
            SessionLock.EnterWriteLock();
            httpContext.Session[CacheId + "_state"] = state;
            SessionLock.ExitWriteLock();
        }
        public string ReadUserStateValue()
        {
            string state = string.Empty;
            SessionLock.EnterReadLock();
            state = (string)httpContext.Session[CacheId + "_state"];
            SessionLock.ExitReadLock();
            return state;
        }
		public void SaveUserTokenValue(string token, DateTime expiration)
		{
			SessionLock.EnterWriteLock();
			httpContext.Session[CacheId] = token;
			httpContext.Session[$"{CacheId}_expiration"] = expiration;
			SessionLock.ExitWriteLock();
		}
		public string ReadUserTokenValue()
		{
			string token = string.Empty;
			SessionLock.EnterReadLock();
			token = (string)httpContext.Session[CacheId];
			SessionLock.ExitReadLock();
			return token;
		}

		public DateTime ReadTokenExpirationValue()
		{
			DateTime expiration = DateTime.MinValue;
			SessionLock.EnterReadLock();
			expiration = (DateTime)httpContext.Session[$"{CacheId}_expiration"];
			SessionLock.ExitReadLock();
			return expiration;
		}

		public void SaveUserCodeValue(string code)
		{
			SessionLock.EnterWriteLock();
			httpContext.Session[CacheId + "_code"] = code;
			SessionLock.ExitWriteLock();
		}
		public string ReadUserCodeValue()
		{
			string code = string.Empty;
			SessionLock.EnterReadLock();
			code = (string)httpContext.Session[CacheId + "_code"];
			SessionLock.ExitReadLock();
			return code;
		}
		public void SaveUserRefreshValue(string refreshCode)
		{
			SessionLock.EnterWriteLock();
			httpContext.Session[CacheId + "_refreshcode"] = refreshCode;
			SessionLock.ExitWriteLock();
		}
		public string ReadUserRefreshValue()
		{
			string refreshCode = string.Empty;
			SessionLock.EnterReadLock();
			refreshCode = (string)httpContext.Session[CacheId + "_refreshcode"];
			SessionLock.ExitReadLock();
			return refreshCode;
		}

	}
}