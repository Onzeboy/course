
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using System.Security.Claims;

namespace course
{
    public static class ExtensionMethods
    {
        public static string getUserId(this ClaimsPrincipal user)
        {
            //returns the current UserID
            if (user.Identity.IsAuthenticated)
                return null;

            ClaimsPrincipal currentUser = user;
            //output
            return currentUser.FindFirst(ClaimTypes.NameIdentifier).Value;
            //</output>
        }
    }
}