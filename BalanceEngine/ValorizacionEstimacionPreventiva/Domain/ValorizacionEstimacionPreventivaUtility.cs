/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reporting Services                           Component : Excel Exporters                       *
*  Assembly : FinancialAccounting.Reporting.dll            Pattern   : IExcelExporter                        *
*  Type     : ValorizacionEstimacionPreventivaUtility      License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Utility methods to manage accounts information for Valorizacion report.                        *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.BalanceEngine {

  /// <summary>Utility methods to manage accounts information for Valorizacion report.</summary>
  internal class ValorizacionEstimacionPreventivaUtility {

    internal string GetMonthNameAndYear(DateTime date) {
      return $"{GetMonthName(date)}_{date.Year}";
    }


    internal string GetMonthName(DateTime date) {
      var months = new string[] {
        "Enero", "Febrero", "Marzo", "Abril",
        "Mayo", "Junio", "Julio", "Agosto",
        "Septiembre", "Octubre", "Noviembre", "Diciembre"
      };

      return months[date.Month - 1];
    }


  } // class ValorizacionEstimacionPreventivaUtility

} // namespace Empiria.FinancialAccounting.BalanceEngine
