/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Interface adapters                      *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Mapper class                            *
*  Type     : TrialBalanceMapper                         License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Methods used to map trial balances.                                                            *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.BalanceEngine.Adapters {

  static internal class TrialBalanceMapper {

    #region Public mappers

    static internal TrialBalanceDto Map(TrialBalance trialBalance) {
      return new TrialBalanceDto {
        Query = trialBalance.Query,
        Columns = trialBalance.DataColumns(),
        Entries = Map(trialBalance.Query, trialBalance.Entries)
      };
    }


    #endregion Public mappers

    #region Helpers


    static private FixedList<ITrialBalanceEntryDto> Map(TrialBalanceQuery query,
                                                        FixedList<ITrialBalanceEntry> list) {
      switch (query.TrialBalanceType) {
        case TrialBalanceType.AnaliticoDeCuentas:

          var analitico = list.Select((x) => AnaliticoDeCuentasMapper.MapEntry((AnaliticoDeCuentasEntry) x));
          return new FixedList<ITrialBalanceEntryDto>(analitico);

        case TrialBalanceType.Balanza:

          var balanza = list.Select((x) => BalanzaTradicionalMapper.MapEntry((TrialBalanceEntry) x, query));
          return new FixedList<ITrialBalanceEntryDto>(balanza);

        case TrialBalanceType.BalanzaConContabilidadesEnCascada:

          var balanzaCascada = list.Select((x) => BalanzaContabilidadesCascadaMapper
                                   .MapEntry((TrialBalanceEntry) x, query));
          return new FixedList<ITrialBalanceEntryDto>(balanzaCascada);

        case TrialBalanceType.BalanzaDiferenciaDiariaPorMoneda:

          var balanzaDifDiaria = list.Select((x) =>
                BalanzaDiferenciaDiariaMonedaMapper.MapEntry((BalanzaDiferenciaDiariaMonedaEntry) x));

          return new FixedList<ITrialBalanceEntryDto>(balanzaDifDiaria);

        case TrialBalanceType.BalanzaDolarizada:

          var balanzaDolarizada = list.Select((x) =>
                BalanzaDolarizadaMapper.MapEntry((BalanzaDolarizadaEntry) x));
          return new FixedList<ITrialBalanceEntryDto>(balanzaDolarizada);

        case TrialBalanceType.BalanzaEnColumnasPorMoneda:

          var currencyMappedItems = list.Select((x) =>
                BalanzaColumnasMonedaMapper.MapEntry((BalanzaColumnasMonedaEntry) x));

          return new FixedList<ITrialBalanceEntryDto>(currencyMappedItems);

        case TrialBalanceType.BalanzaValorizadaComparativa:

          var balanzaComparativa = list.Select((x) => BalanzaComparativaMapper
                                       .MapEntry((BalanzaComparativaEntry) x));

          return new FixedList<ITrialBalanceEntryDto>(balanzaComparativa);

        case TrialBalanceType.GeneracionDeSaldos:

          var mappedItems = list.Select((x) => MapToTrialBalance((TrialBalanceEntry) x, query));
          return new FixedList<ITrialBalanceEntryDto>(mappedItems);

        case TrialBalanceType.ResumenAjusteAnual:

          var resumenAjusteAnual = list.Select((x) =>
                ResumenAjusteAnualMapper.MapEntry((ResumenAjusteAnualEntry) x));

          return new FixedList<ITrialBalanceEntryDto>(resumenAjusteAnual);

        case TrialBalanceType.SaldosPorAuxiliar:

          var saldosPorAuxiliar = list.Select((x) => 
                                  SaldosPorAuxiliarMapper.MapEntry((TrialBalanceEntry) x, query));
          return new FixedList<ITrialBalanceEntryDto>(saldosPorAuxiliar);

        case TrialBalanceType.SaldosPorCuenta:

          var saldosPorCuenta = list.Select((x) => SaldosPorCuentaMapper
                                    .MapEntry((TrialBalanceEntry) x, query));

          return new FixedList<ITrialBalanceEntryDto>(saldosPorCuenta);

        case TrialBalanceType.ValorizacionEstimacionPreventiva:
          
          var valorizacion = list.Select((x) =>
                ValorizacionEstimacionPreventivaMapper.MapEntry((ValorizacionEstimacionPreventivaEntry) x));

          return new FixedList<ITrialBalanceEntryDto>(valorizacion);

        default:
          throw Assertion.EnsureNoReachThisCode(
                $"Unhandled trial balance type {query.TrialBalanceType}.");
      }
    }


    static private TrialBalanceEntryDto MapToTrialBalance(TrialBalanceEntry entry,
                                                          TrialBalanceQuery query) {
      var dto = new TrialBalanceEntryDto();
      SubledgerAccount subledgerAccount = SubledgerAccount.Parse(entry.SubledgerAccountId);

      dto.ItemType = entry.ItemType;
      dto.LedgerUID = entry.Ledger.UID != "Empty" ? entry.Ledger.UID : "";
      dto.LedgerNumber = entry.Ledger.Number;
      if (entry.ItemType == TrialBalanceItemType.Summary ||
          entry.ItemType == TrialBalanceItemType.Entry) {
        dto.LedgerName = entry.Ledger.Name;
      }
      dto.StandardAccountId = entry.Account.Id;
      dto.CurrencyCode = entry.ItemType == TrialBalanceItemType.BalanceTotalConsolidated ? "" :
                         entry.Currency.Code;
      if (subledgerAccount.IsEmptyInstance || subledgerAccount.Number == "0") {
        dto.AccountName = entry.GroupName != "" ? entry.GroupName :
                          entry.Account.Name;
        dto.AccountNumber = entry.GroupNumber != "" ? entry.GroupNumber :
                            !entry.Account.IsEmptyInstance ?
                            entry.Account.Number : "";
      } else {
        dto.AccountName = subledgerAccount.Name;
        dto.AccountNumber = subledgerAccount.Number;
      }
      dto.AccountNumberForBalances = entry.Account.Number;

      if (query.TrialBalanceType == TrialBalanceType.SaldosPorAuxiliar &&
          entry.ItemType == TrialBalanceItemType.Entry && subledgerAccount.IsEmptyInstance) {

        subledgerAccount = SubledgerAccount.Parse(entry.SubledgerAccountIdParent);
        if (!subledgerAccount.IsEmptyInstance) {
          dto.SubledgerAccountNumber = subledgerAccount.Number;
        }
      } else {
        dto.SubledgerAccountNumber = subledgerAccount.Number;
      }

      dto.AccountRole = entry.Account.Role;
      dto.AccountLevel = entry.Account.Level;
      dto.SectorCode = entry.Sector.Code;
      dto.SubledgerAccountId = entry.SubledgerAccountId;
      dto.InitialBalance = entry.InitialBalance;
      dto.Debit = entry.Debit;
      dto.Credit = entry.Credit;
      dto.CurrentBalance = entry.CurrentBalance;
      dto.CurrentBalanceForBalances = entry.CurrentBalance;
      dto.ExchangeRate = entry.ExchangeRate;
      dto.SecondExchangeRate = entry.SecondExchangeRate;
      dto.AverageBalance = entry.AverageBalance;
      if (query.TrialBalanceType == TrialBalanceType.SaldosPorAuxiliar) {
        dto.DebtorCreditor = entry.ItemType == TrialBalanceItemType.Entry ?
                             entry.DebtorCreditor.ToString() : "";

        dto.LastChangeDate = entry.ItemType == TrialBalanceItemType.Entry ?
                             entry.LastChangeDate : ExecutionServer.DateMaxValue;
      } else {
        dto.DebtorCreditor = entry.ItemType == TrialBalanceItemType.Entry ||
                           entry.ItemType == TrialBalanceItemType.Summary ?
                           entry.DebtorCreditor.ToString() : "";

        dto.LastChangeDate = entry.ItemType == TrialBalanceItemType.Entry ||
                           entry.ItemType == TrialBalanceItemType.Summary ||
                           query.TrialBalanceType == TrialBalanceType.BalanzaConContabilidadesEnCascada ?
                           entry.LastChangeDate : ExecutionServer.DateMaxValue;
      }
      dto.LastChangeDateForBalances = dto.LastChangeDate;
      dto.IsParentPostingEntry = entry.IsParentPostingEntry;
      dto.HasAccountStatement = (entry.ItemType == TrialBalanceItemType.Entry ||
                                 entry.ItemType == TrialBalanceItemType.Summary) &&
                                !query.UseDefaultValuation &&
                                query.InitialPeriod.ValuateToCurrrencyUID.Length == 0 &&
                                query.InitialPeriod.ExchangeRateTypeUID.Length == 0;
      dto.ClickableEntry = (entry.ItemType == TrialBalanceItemType.Entry ||
                            entry.ItemType == TrialBalanceItemType.Summary) &&
                            !query.UseDefaultValuation &&
                            query.InitialPeriod.ValuateToCurrrencyUID.Length == 0 &&
                            query.InitialPeriod.ExchangeRateTypeUID.Length == 0;

      return dto;
    }


    #endregion Helpers

  } // class TrialBalanceMapper

} // namespace Empiria.FinancialAccounting.BalanceEngine.Adapters
