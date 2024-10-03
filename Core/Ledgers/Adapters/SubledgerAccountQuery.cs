/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Ledger Management                            Component : Interface adapters                    *
*  Assembly : FinancialAccounting.Core.dll                 Pattern   : Query payload                         *
*  Type     : SubledgerAccountQuery                        License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Query payload used to search subledger accounts (cuentas auxiliares).                          *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.Adapters {

  /// <summary> Query payload used to search subledger accounts (cuentas auxiliares).</summary>
  public class SubledgerAccountQuery {

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


  }  // SubledgerAccountQuery


  static internal class SubledgerAccountQueryExtensions {

    static internal string MapToFilterString(this SubledgerAccountQuery query) {

      string keywordsFilter = BuildKeywordsFilter(query.Keywords);
      string typeFilter     = BuildTypeFilter(query.TypeUID);
      string ledgerFilter   = BuildLedgerFilter(query.LedgerUID);

      var filter = new Filter(keywordsFilter);

      filter.AppendAnd(ledgerFilter);
      filter.AppendAnd(typeFilter);


      if (filter.ToString().Length == 0) {
        filter.AppendOr(SearchExpression.AllRecordsFilter);
      }

      return filter.ToString();
    }


    static private string BuildKeywordsFilter(string keywords) {
      if (keywords.Length == 0) {
        return string.Empty;
      }

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

      var subledgerType = SubledgerType.Parse(typeUID);

      return $"ID_TIPO_MAYOR_AUXILIAR = {subledgerType.Id}";
    }

  }  // class SubledgerAccountQueryExtensions

}  // namespace Empiria.FinancialAccounting.Adapters
