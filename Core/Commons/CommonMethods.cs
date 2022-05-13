/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Accounting                       Component : Common Types                            *
*  Assembly : FinancialAccounting.Core.dll               Pattern   : Static methods library                  *
*  Type     : CommonMethods                              License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Static methods library for financial accounting.                                               *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Data;

namespace Empiria.FinancialAccounting {

  /// <summary>Static methods library for financial accounting.</summary>
  static public class CommonMethods {

    #region Public methods

    static public string FormatSqlDbDate(DateTime date) {
      string dateAsString = date.Date.ToString("yyyy-MM-dd");

      return $"TO_DATE('{dateAsString}', 'yyyy-MM-dd')";
    }


    static public string FormatSqlDbDateTime(DateTime date) {
      string dateTimeAsString = date.ToString("yyyy-MM-dd HH:mm:ss");

      return $"TO_DATE('{dateTimeAsString}', 'yyyy-MM-dd hh24:mi:ss')";
    }


    static public long GetNextObjectId(string sequenceName) {
      var sql = $"SELECT {sequenceName}.NEXTVAL FROM DUAL";

      var operation = DataOperation.Parse(sql);

      return Convert.ToInt64(DataReader.GetScalar<decimal>(operation));
    }

    #endregion Public methods

  }  // class CommonMethods

}  // namespace Empiria.FinancialAccounting
