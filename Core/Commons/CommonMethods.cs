/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Accounting                       Component : Root Types                              *
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

    static public string FormatSqlDate(DateTime date) {
      return $"{date.Date.ToString("dd/MMM/yyyy")}";
    }


    static public long GetNextObjectId(string sequenceName) {
      var sql = $"SELECT {sequenceName}.NEXTVAL FROM DUAL";

      var operation = DataOperation.Parse(sql);

      return Convert.ToInt64(DataReader.GetScalar<decimal>(operation));
    }

    #endregion Public methods

  }  // class CommonMethods

}  // namespace Empiria.FinancialAccounting
