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
using System.Linq;

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
                   List<TrialBalanceEntry> entries, FixedList<Account> accounts) {

      var mapped = entries.Select(x => MapToLockedUpEntry(x, accounts));

      return new FixedList<SaldosEncerradosEntryDto>(mapped);
    }

    #endregion Methods

    #region Helpers

    static private void AccountClauses(SaldosEncerradosEntryDto dto,
                                       TrialBalanceEntry entry,
                                       Account account) {

      if (entry.SubledgerAccountNumber.Length > 1) {

        dto.AccountNumber = entry.Account.Number;
        dto.SubledgerAccount = entry.SubledgerAccountNumber;

      } else {
        dto.AccountNumber = entry.Account.Number;
        dto.SubledgerAccount = "";
      }
    }


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

      var dto = new SaldosEncerradosEntryDto();
      AccountClauses(dto, entry, account);
      RoleClauses(dto, entry, account);
      dto.StandardAccountId = entry.Account.Id;
      dto.CurrencyCode = entry.Currency.Code;
      dto.LedgerUID = entry.Ledger.UID;
      dto.LedgerNumber = entry.Ledger.Number;
      dto.LedgerName = entry.Ledger.Name;
      dto.RoleChangeDate = account.EndDate;
      dto.RoleChange = $"{account.Role}-{entry.Account.Role}";
      dto.ItemName = entry.Account.Name;
      dto.SectorCode = entry.Sector.Code;
      dto.LockedBalance = (decimal) entry.CurrentBalance;
      dto.LastChangeDate = entry.LastChangeDate;
      dto.NewRole = entry.Account.Role.ToString();
      dto.DebtorCreditor = entry.DebtorCreditor.ToString();

      return dto;
    }


    static private void RoleClauses(SaldosEncerradosEntryDto dto,
                                    TrialBalanceEntry entry, Account account) {

      if (account.Role == AccountRole.Detalle) {

        dto.ItemType = TrialBalanceItemType.Summary;
        dto.IsCancelable = true;

      } else {

        dto.ItemType = entry.ItemType;
        if (entry.ItemType == TrialBalanceItemType.Entry) {
          dto.IsCancelable = true;
        }

      }
    }

    #endregion Helpers

  } // class SaldosEncerradosMapper

} // namespace Empiria.FinancialAccounting.BalanceEngine.Adapters
