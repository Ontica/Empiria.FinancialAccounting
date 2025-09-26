/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Interface adapters                      *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Mapper class                            *
*  Type     : SaldosEncerradosMapper                     License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Mapper for saldos encerrados.                                                                  *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using System.Collections.Generic;

using Empiria.DynamicData;

namespace Empiria.FinancialAccounting.BalanceEngine.Adapters {

  /// <summary>Mapper for saldos encerrados.</summary>
  static internal class SaldosEncerradosMapper {

    #region Methods

    static public SaldosEncerradosDto Map(FixedList<SaldosEncerradosBaseEntryDto> mappedEntries) {
      return new SaldosEncerradosDto {
        Columns = DataColumns(),
        Entries = mappedEntries
      };
    }


    static public FixedList<SaldosEncerradosEntryDto> MergeBalancesIntoLockedBalanceEntries(
                   FixedList<TrialBalanceEntry> entries, FixedList<Account> accounts) {

      var mapped = entries.Select(x => MapToLockedUpEntry(x, accounts));

      return new FixedList<SaldosEncerradosEntryDto>(mapped);
    }

    #endregion Methods

    #region Helpers

    static private FixedList<DataTableColumn> DataColumns() {
      var columns = new List<DataTableColumn> {
        new DataTableColumn("ledgerNumber", "Deleg", "text"),
        new DataTableColumn("ledgerName", "Delegación", "text"),
        new DataTableColumn("currencyCode", "Mon", "text"),
        new DataTableColumn("accountNumber", "Cuenta", "text"),
        new DataTableColumn("itemName", "Nombre", "text"),
        new DataTableColumn("sectorCode", "Sector", "text"),
        new DataTableColumn("subledgerAccount", "Auxiliar", "text"),
        new DataTableColumn("lockedBalance", "Saldo encerrado", "decimal"),
        new DataTableColumn("roleChangeDate", "Fecha cambio Rol", "date"),
        new DataTableColumn("roleChange", "Rol", "text")
      };

      return columns.ToFixedList();
    }


    static private SaldosEncerradosEntryDto MapToLockedUpEntry(
                   TrialBalanceEntry entry, FixedList<Account> accounts) {

      var account = accounts.Find(a => a.Number == entry.Account.Number);

      return new SaldosEncerradosEntryDto {

        ItemType = entry.ItemType,
        AccountNumber = entry.Account.Number,
        SubledgerAccount = entry.SubledgerAccountNumber.Length > 1 ?
                                  entry.SubledgerAccountNumber : string.Empty,
        StandardAccountId = entry.Account.Id,
        CurrencyCode = entry.Currency.Code,
        LedgerUID = entry.Ledger.UID,
        LedgerNumber = entry.Ledger.Number,
        LedgerName = entry.Ledger.Name,
        RoleChangeDate = account.EndDate,
        RoleChange = $"{account.Role}-{entry.Account.Role}",
        ItemName = entry.Account.Name,
        SectorCode = entry.Sector.Code,
        LockedBalance = entry.CurrentBalance,
        LastChangeDate = entry.LastChangeDate,
        PreviousRole = account.Role.ToString(),
        NewRole = entry.Account.Role.ToString(),
        DebtorCreditor = entry.DebtorCreditor.ToString(),
        IsCancelable = (entry.ItemType == TrialBalanceItemType.Entry &&
                        entry.CurrentBalance != 0)
      };
    }

    #endregion Helpers

  } // class SaldosEncerradosMapper

} // namespace Empiria.FinancialAccounting.BalanceEngine.Adapters
