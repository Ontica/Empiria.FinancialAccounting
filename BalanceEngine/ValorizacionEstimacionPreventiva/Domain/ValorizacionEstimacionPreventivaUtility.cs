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

      if (date.Month == 1) {
        return $"Enero_{date.Year}";
      }
      if (date.Month == 2) {
        return $"Febrero_{date.Year}";
      }
      if (date.Month == 3) {
        return $"Marzo_{date.Year}";
      }
      if (date.Month == 4) {
        return $"Abril_{date.Year}";
      }
      if (date.Month == 5) {
        return $"Mayo_{date.Year}";
      }
      if (date.Month == 6) {
        return $"Junio_{date.Year}";
      }
      if (date.Month == 7) {
        return $"Julio_{date.Year}";
      }
      if (date.Month == 8) {
        return $"Agosto_{date.Year}";
      }
      if (date.Month == 9) {
        return $"Septiembre_{date.Year}";
      }
      if (date.Month == 10) {
        return $"Octubre_{date.Year}";
      }
      if (date.Month == 11) {
        return $"Noviembre_{date.Year}";
      }
      if (date.Month == 12) {
        return $"Diciembre_{date.Year}";
      }

      return "No se encontró información";
    }


    internal string GetMonthName(DateTime date) {

      if (date.Month == 1) {
        return $"Enero";
      }
      if (date.Month == 2) {
        return $"Febrero";
      }
      if (date.Month == 3) {
        return $"Marzo";
      }
      if (date.Month == 4) {
        return $"Abril";
      }
      if (date.Month == 5) {
        return $"Mayo";
      }
      if (date.Month == 6) {
        return $"Junio";
      }
      if (date.Month == 7) {
        return $"Julio";
      }
      if (date.Month == 8) {
        return $"Agosto";
      }
      if (date.Month == 9) {
        return $"Septiembre";
      }
      if (date.Month == 10) {
        return $"Octubre";
      }
      if (date.Month == 11) {
        return $"Noviembre";
      }
      if (date.Month == 12) {
        return $"Diciembre";
      }
      return "No se encontró información";
    }


  } // class ValorizacionEstimacionPreventivaUtility

} // namespace Empiria.FinancialAccounting.BalanceEngine
