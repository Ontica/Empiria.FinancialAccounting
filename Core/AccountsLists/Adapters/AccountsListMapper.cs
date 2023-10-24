/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Accounts Lists                             Component : Interface adapters                      *
*  Assembly : Empiria.FinancialAccounting.dll            Pattern   : Mapper class                            *
*  Type     : AccountsListMapper                         License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Methods used to map accounts lists.                                                            *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;

using Empiria.FinancialAccounting.AccountsLists.SpecialCases;

namespace Empiria.FinancialAccounting.AccountsLists.Adapters {

  /// <summary>Methods used to map accounts lists.</summary>
  static internal class AccountsListMapper {

    #region Public mappers

    static internal AccountsListDto Map(AccountsList list, string keywords) {

      return new AccountsListDto {
        UID = list.UID,
        Name = list.Name,
        Columns = list.DataTableColumns,
        Entries = MapEntries(list, keywords)
      };
    }

    #endregion Public mappers

    #region Private methods


    static private FixedList<AccountsListItemDto> MapEntries(AccountsList list, string keywords) {
      FixedList<AccountsListItemDto> entries;
      switch (list.UID) {
        case "ConciliacionDerivados":
          entries = new FixedList<AccountsListItemDto>(MapList(list.GetItems<ConciliacionDerivadosListItem>(keywords)));
          break;
        case "SwapsCobertura":
          entries = new FixedList<AccountsListItemDto>(MapList(list.GetItems<SwapsCoberturaListItem>(keywords)));
          break;
        case "DepreciacionActivoFijo":
          entries = new FixedList<AccountsListItemDto>(MapList(list.GetItems<DepreciacionActivoFijoListItem>(keywords)));
          break;
        default:
          throw new NotImplementedException($"Unrecognized accounts list UID {list.UID}.");
      }

      return entries;
    }

    static private IEnumerable<ConciliacionDerivadosListItemDto> MapList(FixedList<ConciliacionDerivadosListItem> entries) {
      var mapped = entries.Select(x => MapEntry(x));

      return new FixedList<ConciliacionDerivadosListItemDto>(mapped);
    }

    static private IEnumerable<SwapsCoberturaListItemDto> MapList(FixedList<SwapsCoberturaListItem> entries) {
      var mapped = entries.Select(x => MapEntry(x));

      return new FixedList<SwapsCoberturaListItemDto>(mapped);
    }


    static private IEnumerable<DepreciacionActivoFijoListItemDto> MapList(FixedList<DepreciacionActivoFijoListItem> entries) {
      var mapped = entries.Select(x => MapEntry(x));

      return new FixedList<DepreciacionActivoFijoListItemDto>(mapped);
    }


    static internal ConciliacionDerivadosListItemDto MapEntry(ConciliacionDerivadosListItem item) {
      return new ConciliacionDerivadosListItemDto {
        UID = item.UID,
        AccountUID = item.Account.UID,
        AccountNumber = item.Account.Number,
        AccountName = item.Account.Name,
      };
    }


    static internal SwapsCoberturaListItemDto MapEntry(SwapsCoberturaListItem item) {
      return new SwapsCoberturaListItemDto {
        UID = item.UID,
        SubledgerAccountId = item.SubledgerAccount.Id,
        SubledgerAccountName = item.SubledgerAccount.Name,
        SubledgerAccountNumber = item.SubledgerAccount.Number,
        Classification = item.Classification
      };
    }


    static internal DepreciacionActivoFijoListItemDto MapEntry(DepreciacionActivoFijoListItem item) {
      var value = new DepreciacionActivoFijoListItemDto {
        UID = item.UID,
        AuxiliarHistoricoId = item.AuxiliarHistorico.Id,
        AuxiliarHistoricoNombre = item.AuxiliarHistorico.Name,
        AuxiliarHistorico = item.AuxiliarHistorico.Number,
        NumeroInventario = item.NumeroInventario,
        NumeroDelegacion = item.Ledger.Number,
        Delegacion = item.Ledger.Name,
        DelegacionId = item.Ledger.Id,
        FechaDepreciacion = item.FechaDepreciacion,
        InicioDepreciacion = item.InicioDepreciacion,
        MesesDepreciacion = item.MesesDepreciacion
      };

      if (!item.AuxiliarRevaluacion.IsEmptyInstance) {
        value.AuxiliarRevaluacion = item.AuxiliarRevaluacion.Number;
        value.AuxiliarRevaluacionId = item.AuxiliarRevaluacion.Id;
        value.AuxiliarRevaluacionNombre = item.AuxiliarRevaluacion.Name;
      }

      return value;
    }

    #endregion Private methods

  } // class AccountsListMapper

} // Empiria.FinancialAccounting.Adapters
