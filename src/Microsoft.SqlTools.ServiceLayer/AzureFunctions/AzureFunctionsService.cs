﻿//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
//
using System;
using System.Threading.Tasks;
using Microsoft.SqlTools.Hosting.Protocol;
using Microsoft.SqlTools.ServiceLayer.AzureFunctions.Contracts;
using Microsoft.SqlTools.ServiceLayer.Connection;
using Microsoft.SqlTools.ServiceLayer.Hosting;
using Microsoft.SqlTools.ServiceLayer.TaskServices;
using Microsoft.SqlTools.ServiceLayer.Utility;

namespace Microsoft.SqlTools.ServiceLayer.AzureFunctions
{
    /// <summary>
    /// Main class for Azure Functions service
    /// </summary>
    class AzureFunctionsService
    {
        private static ConnectionService connectionService = null;
        private SqlTaskManager sqlTaskManagerInstance = null;
        private static readonly Lazy<AzureFunctionsService> instance = new Lazy<AzureFunctionsService>(() => new AzureFunctionsService());
       
        /// <summary>
        /// Gets the singleton instance object
        /// </summary>
        public static AzureFunctionsService Instance
        {
            get { return instance.Value; }
        }

        /// <summary>
        /// Initializes the service instance
        /// </summary>
        /// <param name="serviceHost"></param>
        public void InitializeService(ServiceHost serviceHost)
        {
            serviceHost.SetRequestHandler(InsertSqlBindingRequest.Type, this.HandleInsertSqlBindingRequest);
            serviceHost.SetRequestHandler(GetAzureFunctionsRequest.Type, this.HandleGetAzureFunctionsRequest);
        }

        /// <summary>
        /// Handles request to insert sql binding into Azure Functions
        /// </summary>
        /// <returns></returns>
        public async Task HandleInsertSqlBindingRequest(InsertSqlBindingParams parameters, RequestContext<ResultStatus> requestContext)
        {
            try
            {
                InsertSqlBindingOperation operation = new InsertSqlBindingOperation(parameters);
                ResultStatus result = operation.AddBinding();

                await requestContext.SendResult(result);
            }
            catch (Exception e)
            {
                await requestContext.SendError(e);
            }
        }

        /// <summary>
        /// Handles request to get azure functions in a file
        /// </summary>
        /// <returns></returns>
        public async Task HandleGetAzureFunctionsRequest(GetAzureFunctionsParams parameters, RequestContext<GetAzureFunctionsResult> requestContext)
        {
            try
            {
                GetAzureFunctionsOperation operation = new GetAzureFunctionsOperation(parameters);
                GetAzureFunctionsResult result = operation.GetAzureFunctions();

                await requestContext.SendResult(result);
            }
            catch (Exception e)
            {
                await requestContext.SendError(e);
            }
        }

        private SqlTaskManager SqlTaskManagerInstance
        {
            get
            {
                if (sqlTaskManagerInstance == null)
                {
                    sqlTaskManagerInstance = SqlTaskManager.Instance;
                }
                return sqlTaskManagerInstance;
            }
            set
            {
                sqlTaskManagerInstance = value;
            }
        }

        /// <summary>
        /// Internal for testing purposes only
        /// </summary>
        internal static ConnectionService ConnectionServiceInstance
        {
            get
            {
                if (connectionService == null)
                {
                    connectionService = ConnectionService.Instance;
                }
                return connectionService;
            }
            set
            {
                connectionService = value;
            }
        }
    }
}