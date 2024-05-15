using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace WeatherApp
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            config.EnableCors();
            // Route configuration for LoginController
            config.Routes.MapHttpRoute(
                name: "Login",
                routeTemplate: "api/login",
                defaults: new { controller = "Login", action = "Login" }
            );

            // Route configuration for RegisterController
            config.Routes.MapHttpRoute(
                name: "Register",
                routeTemplate: "api/register",
                defaults: new { controller = "Register", action = "Register" }
            );

            // Route configuration for EditUserController
            config.Routes.MapHttpRoute(
                name: "EditUserDetails",
                routeTemplate: "api/edituser/{userId}",
                defaults: new { controller = "EditUser", action = "EditUserDetails", userId = RouteParameter.Optional }
            );

            // Route configuration for UserInfoController
            config.Routes.MapHttpRoute(
                name: "GetUserById",
                routeTemplate: "api/user/{userId}",
                defaults: new { controller = "UserInfo", action = "GetUserById", userId = RouteParameter.Optional }
            );
        }
    }
}
