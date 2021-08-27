#region header
// /*********************************************************************************************/
// Project :DaoDatabase
// FileName : IDbFactory.cs
// Time Create : 1:57 PM 28/12/2015
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/
#endregion

using System;

namespace DaoDatabase
{
    internal interface IDbFactory:IDisposable
    {
        Reponsitory CreateContext(string name = "");
    }
}