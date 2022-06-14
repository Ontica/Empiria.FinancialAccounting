/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Interface adapters                      *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Mapper class                            *
*  Type     : BalanzaComparativaMapper                         License   : Please read LICENSE.txt file      *
*                                                                                                            *
*  Summary  : Methods used to map balanza comparativa.                                                       *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;

namespace Empiria.FinancialAccounting.BalanceEngine.Adapters {


  /// <summary></summary>
  static internal class BalanzaComparativaMapper {


    #region Public methods

    static internal BalanzaComparativaDto Map(TrialBalanceQuery query,
                                              FixedList<BalanzaComparativaEntry> entries) {
      return new BalanzaComparativaDto {
        Query = query,
        Columns = DataColumns(query),
        Entries = entries.Select(x => MapEntry(x))
                         .ToFixedList()
      };
    }


    #endregion Public methods


    #region Private methods


    static public FixedList<DataTableColumn> DataColumns(TrialBalanceQuery Query) {
      List<DataTableColumn> columns = new List<DataTableColumn>();

      if (Query.ReturnLedgerColumn) {
        columns.Add(new DataTableColumn("ledgerNumber", "Cont", "text"));
      }
      columns.Add(new DataTableColumn("currencyCode", "Mon", "text"));
      columns.Add(new DataTableColumn("accountParent", "Cta", "text-nowrap"));
      columns.Add(new DataTableColumn("accountNumber", "Cuenta", "text-nowrap"));
      columns.Add(new DataTableColumn("sectorCode", "Sct", "text"));
      columns.Add(new DataTableColumn("subledgerAccountNumber", "Auxiliar", "text-nowrap"));
      columns.Add(new DataTableColumn("subledgerAccountName", "Nombre", "text"));

      columns.Add(new DataTableColumn("firstTotalBalance", $"{Query.InitialPeriod.FromDate:MMM_yyyy}",
                                                            "decimal"));
      columns.Add(new DataTableColumn("firstExchangeRate", "Tc_Ini", "decimal", 6));
      columns.Add(new DataTableColumn("firstValorization", $"{Query.InitialPeriod.FromDate:MMM}_VAL_A",
                                                            "decimal"));

      columns.Add(new DataTableColumn("debit", "Cargos", "decimal"));
      columns.Add(new DataTableColumn("credit", "Abonos", "decimal"));
      columns.Add(new DataTableColumn("secondTotalBalance", $"{Query.FinalPeriod.FromDate:MMM_yyyy}",
                                                             "decimal"));
      columns.Add(new DataTableColumn("secondExchangeRate", "Tc_Fin", "decimal", 6));
      columns.Add(new DataTableColumn("secondValorization", $"{Query.FinalPeriod.FromDate:MMM}_VAL_B",
                                                             "decimal"));

      columns.Add(new DataTableColumn("accountName", "Nom_Cta", "text"));
      columns.Add(new DataTableColumn("debtorCreditor", "Nat", "text"));
      columns.Add(new DataTableColumn("variation", "Variación", "decimal"));
      columns.Add(new DataTableColumn("variationByER", "Variación por TC", "decimal"));
      columns.Add(new DataTableColumn("realVariation", "Variación por TC", "decimal"));
      if (Query.WithAverageBalance) {
        columns.Add(new DataTableColumn("averageBalance", "Saldo promedio", "decimal"));
        columns.Add(new DataTableColumn("lastChangeDate", "Último movimiento", "date"));
      }

      return columns.ToFixedList();
    }


    static public BalanzaComparativaEntryDto MapEntry(BalanzaComparativaEntry entry) {
      var dto = new BalanzaComparativaEntryDto();
      
      dto.ItemType = entry.ItemType;
      dto.AccountRole = entry.Account.Role;
      dto.AccountLevel = entry.Account.Level;
      dto.DebtorCreditor = entry.Account.DebtorCreditor;
      dto.LedgerUID = entry.Ledger.UID != "Empty" ? entry.Ledger.UID : "";
      dto.LedgerNumber = entry.Ledger.Number;
      dto.LedgerName = entry.Ledger.Name;
      dto.StandardAccountId = entry.Account.Id;
      dto.CurrencyCode = entry.Currency.Code;
      dto.SectorCode = entry.Sector.Code;
      dto.AccountParent = entry.Account.FirstLevelAccountNumber;
      dto.AccountNumber = entry.Account.Number;
      dto.AccountName = entry.Account.Name;
      dto.AccountNumberForBalances = entry.Account.Number;

      dto.SubledgerAccountId = entry.SubledgerAccountId;
      dto.SubledgerAccountNumber = entry.SubledgerAccountNumber;
      dto.SubledgerAccountName = entry.SubledgerAccountNumber != String.Empty ?
                                 entry.SubledgerAccountName : entry.Account.Name;

      dto.FirstTotalBalance = entry.FirstTotalBalance;
      dto.FirstExchangeRate = entry.FirstExchangeRate;
      dto.FirstValorization = entry.FirstValorization;

      dto.Debit = entry.Debit;
      dto.Credit = entry.Credit;
      dto.SecondTotalBalance = entry.SecondTotalBalance;
      dto.SecondExchangeRate = entry.SecondExchangeRate;
      dto.SecondValorization = entry.SecondValorization;

      dto.Variation = entry.Variation;
      dto.VariationByER = entry.VariationByER;
      dto.RealVariation = entry.RealVariation;
      dto.AverageBalance = entry.AverageBalance;
      dto.LastChangeDate = entry.LastChangeDate;

      return dto;
    }


    #endregion Private methods


  } // class BalanzaComparativaMapper 

} // namespace Empiria.FinancialAccounting.BalanceEngine.Adapters
