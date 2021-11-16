/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Ledger Management                            Component : Interface adapters                    *
*  Assembly : FinancialAccounting.Core.dll                 Pattern   : Command payload                       *
*  Type     : SearchSubledgerAccountCommand                License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Command payload used to search subledger accounts (cuentas auxiliares).                        *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.Adapters {

  /// <summary>Command payload used to search subledger accounts (cuentas auxiliares).</summary>
  public class SearchSubledgerAccountCommand {

    public string AccountsChartUID {
      get; set;
    }


    public string LedgerUID {
      get; set;
    } = string.Empty;


    public string TypeUID {
      get; set;
    } = string.Empty;


    public string[] Lists {
      get; set;
    } = new string[0];


    public string Keywords {
      get; set;
    } = string.Empty;


  }  // SearchSubledgerAccountCommand


  static internal class SearchSubledgerAccountCommandExtensions {

    static internal AccountsChart AccountsChart(this SearchSubledgerAccountCommand command) {
      return FinancialAccounting.AccountsChart.Parse(command.AccountsChartUID);
    }


    static internal string BuildFilter(this SearchSubledgerAccountCommand command) {
      string keywordsFilter = BuildKeywordsFilter(command.Keywords);

      string typeFilter = BuildTypeFilter(command.TypeUID);
      string ledgerFilter = BuildLedgerFilter(command.LedgerUID);

      var filter = new Filter(keywordsFilter);

      filter.AppendAnd(ledgerFilter);
      filter.AppendAnd(typeFilter);

      return filter.ToString();
    }


    static private string BuildKeywordsFilter(string keywords) {
      return SearchExpression.ParseAndLikeKeywords("KEYWORDS_CUENTA_AUXILIAR", keywords);
    }


    static private string BuildLedgerFilter(string ledgerUID) {
      if (ledgerUID.Length == 0) {
        return string.Empty;
      }

      var ledger = Ledger.Parse(ledgerUID);

      return $"(ID_MAYOR = {ledger.Id} OR ID_MAYOR_ADICIONAL = {ledger.Id})";
    }


    static private string BuildTypeFilter(string typeUID) {
      if (typeUID.Length == 0) {
        return string.Empty;
      }

      return $"ID_TIPO_MAYOR_AUXILIAR = '{typeUID}'";
    }

  }  // class SearchSubledgerAccountCommandExtensions


}  // namespace Empiria.FinancialAccounting.Adapters
