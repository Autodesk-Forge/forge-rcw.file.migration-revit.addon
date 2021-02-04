using System;
using System.Text;
using System.Net;

using Autodesk.Forge;
using Autodesk.Forge.Client;
using System.Threading;

namespace Revit.SDK.Samples.CloudAPISample.CS.Forge
{

    class ThreeLeggedToken
    {
        private static bool _threeLeggedTokenInitialized = false;
        private static ThreeLeggedApi _threeLeggedApi = new ThreeLeggedApi();
        private static string _threeLeggedToken = null;
        private static DateTime _dt;

        // Declare a local web listener to wait for the oAuth callback on the local machine.
        // Please read this article to configure your local machine properly
        // http://stackoverflow.com/questions/4019466/httplistener-access-denied
        //   ex: netsh http add urlacl url=http://+:3006/oauth user=cyrille
        // Embedded webviews are strongly discouraged for oAuth - https://developers.google.com/identity/protocols/OAuth2InstalledApp
        private static HttpListener _httpListener = null;

        private static readonly Scope[] _scope = new Scope[] { Scope.DataRead, Scope.DataWrite };


        private static string FORGE_CLIENT_ID = "z8fGD7sOz27dGsiG1g3df2nelkXchTAV"; /*Environment.GetEnvironmentVariable("FORGE_CLIENT_ID") ?? "your_client_id"*/
        private static string FORGE_CLIENT_SECRET = "B7f971bb0dfb54c7";/* Environment.GetEnvironmentVariable("FORGE_CLIENT_SECRET") ?? "your_client_secret";*/
        private static string FORGE_CALLBACK = "http://localhost:3000/api/forge/callback/oauth";/*Environment.GetEnvironmentVariable("FORGE_CALLBACK") ?? "http://localhost:" + PORT + "/oauth";*/


        internal delegate void NewBearerDelegate( );

        /// <summary>
        /// 
        /// </summary>
        public class TokenData
        {
            public NewBearerDelegate callback = null;
            public dynamic control = null;
        }

        /// <summary>
        /// 
        /// </summary>
        public static void GenerateToken( TokenData cbData )
        {
            _threeLeggedTokenInitialized = false;
            try
            {
                if (!HttpListener.IsSupported)
                {
                    return;
                }

                // Initialize our web listerner
                _httpListener = new HttpListener();
                _httpListener.Prefixes.Add(FORGE_CALLBACK.Replace("localhost", "+") + "/");
                _httpListener.Start();
                
                IAsyncResult result = _httpListener.BeginGetContext(_3leggedAsyncWaitForCode, cbData);

                // Generate a URL page that asks for permissions for the specified scopes, and call our default web browser.
                string oauthUrl = _threeLeggedApi.Authorize(FORGE_CLIENT_ID, oAuthConstants.CODE, FORGE_CALLBACK, _scope);
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(oauthUrl));
            }
            catch (Exception e )
            {
                Console.Write(e);
            }
        }


        internal static async void _3leggedAsyncWaitForCode(IAsyncResult ar)
        {
            try
            {
                // Our local web listener was called back from the Autodesk oAuth server
                // That means the user logged properly and granted our application access
                // for the requested scope.
                // Let's grab the code fron the URL and request or final access_token

                var context = _httpListener.EndGetContext(ar);
                string code = context.Request.QueryString[oAuthConstants.CODE];

                // The code is only to tell the user, he can close is web browser and return
                // to this application.
                var responseString = "<html><body>You can now close this window!</body></html>";
                byte[] buffer = Encoding.UTF8.GetBytes(responseString);
                var response = context.Response;
                response.ContentType = "text/html";
                response.ContentLength64 = buffer.Length;
                response.StatusCode = 200;
                response.OutputStream.Write(buffer, 0, buffer.Length);
                response.OutputStream.Close();

                // Now request the final access_token
                if (!string.IsNullOrEmpty(code))
                {
                    // Call the asynchronous version of the 3-legged client with HTTP information
                    // HTTP information will help you to verify if the call was successful as well
                    // as read the HTTP transaction headers.
                    ApiResponse<dynamic> bearer = await _threeLeggedApi.GettokenAsyncWithHttpInfo(FORGE_CLIENT_ID, FORGE_CLIENT_SECRET, oAuthConstants.AUTHORIZATION_CODE, code, FORGE_CALLBACK);
                    if (bearer.StatusCode != 200 || bearer.Data == null)
                    {
                        throw new Exception("Request failed! ");
                    }
                    // The call returned successfully and you got a valid access_token.
                    _threeLeggedToken = bearer.Data.access_token;
                    _dt = DateTime.Now;
                    _threeLeggedTokenInitialized = true;
                }
                TokenData tokenData = (TokenData)ar.AsyncState;
                if (tokenData != null )
                {
                    tokenData.control.Dispatcher.Invoke(tokenData.callback); 
                }

            }
            catch (Exception ex )
            {
                Console.WriteLine(ex);
                _threeLeggedTokenInitialized = false;

            }
            finally
            {
                _httpListener.Stop();
            }
        }

        /// <summary>
        /// 
        /// 
        /// </summary>
        /// <returns></returns>
        public static string GetToken()
        {
            if (_threeLeggedToken == null || ((DateTime.Now - _dt) > TimeSpan.FromMinutes(30)))
            {
                GenerateToken(null);
                while (!TokenInitialized)
                {
                    Thread.Sleep(2000);
                }
                _dt = DateTime.Now;
                return _threeLeggedToken;
            }
            else return _threeLeggedToken;
        }

        /// <summary>
        /// 
        /// </summary>
        public static bool TokenInitialized
        {
            get { return _threeLeggedTokenInitialized; }
        }
    }
}
