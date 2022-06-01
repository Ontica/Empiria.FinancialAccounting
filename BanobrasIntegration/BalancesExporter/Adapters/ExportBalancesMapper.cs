/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Banobras Integration Services                Component : Balances Exporter                     *
*  Assembly : FinancialAccounting.BanobrasIntegration.dll  Pattern   : Mapper class                          *
*  Type     : ExportBalancesMapper                         License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Maps trial balances entries to ExportedBalancesDto structures.                                 *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.FinancialAccounting.BalanceEngine;
using Empiria.FinancialAccounting.BalanceEngine.Adapters;

using Empiria.FinancialAccounting.BanobrasIntegration.BalancesExporter.Data;

namespace Empiria.FinancialAccounting.BanobrasIntegration.BalancesExporter.Adapters {

  static public class ExportBalancesMapper {

    static public FixedList<ExportedBalancesDto> MapToExportedBalances(ExportBalancesCommand command,
                                                                       TrialBalanceDto trialBalance) {
      FixedList<BalanzaTradicionalEntryDto> entries = GetEntriesToBeExported(trialBalance);

      return new FixedList<ExportedBalancesDto>(entries.Select(x => MapTrialBalanceEntry(command, x)));
    }


    static private FixedList<BalanzaTradicionalEntryDto> GetEntriesToBeExported(TrialBalanceDto trialBalance) {
      var list = trialBalance.Entries.FindAll(x => x.ItemType == TrialBalanceItemType.Entry ||
                                                   x.ItemType == TrialBalanceItemType.Summary)
                                      .Select(x => (BalanzaTradicionalEntryDto) x);

      return list.ToFixedList();
    }


    static private ExportedBalancesDto MapTrialBalanceEntry(ExportBalancesCommand command,
                                                            BalanzaTradicionalEntryDto entry) {
      var account = StandardAccount.Parse(entry.StandardAccountId);
      var subledgerAccount = SubledgerAccount.Parse(entry.SubledgerAccountId);

      var calificaMoneda = CalificacionMoneda.TryParse(account.Number,
                                                       entry.SectorCode,
                                                       subledgerAccount.Number);
      return new ExportedBalancesDto {
        Fecha = command.ToDate,
        Area = "AREA",
        Moneda = 1,
        NumeroMayor = command.BreakdownLedgers ? Ledger.Parse(entry.LedgerUID).Id.ToString(): "CONSOLIDADO",
        Cuenta = account.Number,
        Sector = entry.SectorCode,
        Auxiliar = subledgerAccount.IsEmptyInstance ? "0" : subledgerAccount.Number,
        FechaUltimoMovimiento = entry.LastChangeDate,
        Saldo = (decimal)entry.CurrentBalance,
        MonedaOrigen = Currency.Parse(entry.CurrencyCode).Id,
        NaturalezaCuenta = account.DebtorCreditor == DebtorCreditorType.Deudora ? 1 : -1,
        SaldoPromedio = Math.Round((decimal) entry.AverageBalance, 2),
        MontoDebito = entry.Debit,
        MontoCredito = entry.Credit,
        SaldoAnterior = entry.InitialBalance,
        Empresa = command.AccountsChartId,
        CalificaMoneda = calificaMoneda != null ? calificaMoneda.CalificaSaldo.ToString() : "null"
      };
    }

  }  // class ExportBalancesMapper

}  // namespace Empiria.FinancialAccounting.BanobrasIntegration.BalancesExporter.Adapters
