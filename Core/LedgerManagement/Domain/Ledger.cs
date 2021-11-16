/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Ledger Management                          Component : Domain Layer                            *
*  Assembly : FinancialAccounting.Core.dll               Pattern   : Aggregate root                          *
*  Type     : Ledger                                     License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Holds information about an accounting ledger book.                                             *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using Empiria.Contacts;

using Empiria.FinancialAccounting.Data;

namespace Empiria.FinancialAccounting {

  /// <summary>Holds information about an accounting ledger book.</summary>
  public class Ledger : BaseObject, INamedEntity {

    #region Constructors and parsers

    protected Ledger() {
      // Required by Empiria Framework.
    }


    static public Ledger Parse(int id) {
      return BaseObject.ParseId<Ledger>(id);
    }

    static public Ledger Parse(string uid) {
      return BaseObject.ParseKey<Ledger>(uid);
    }

    static public Ledger Empty => BaseObject.ParseEmpty<Ledger>();


    #endregion Constructors and parsers

    #region Public properties

    [DataField("ID_TIPO_CUENTAS_STD", ConvertFrom = typeof(long))]
    private int _accountsChartId = 0;


    public AccountsChart AccountsChart {
      get {
        return AccountsChart.Parse(_accountsChartId);
      }
    }


    [DataField("NOMBRE_MAYOR")]
    public string Name {
      get; private set;
    } = string.Empty;


    [DataField("NUMERO_MAYOR")]
    public string Number {
      get; private set;
    } = string.Empty;


    public string FullName {
      get {
        if (this.Subnumber.Length == 0) {
          return $"({this.Number}) {this.Name}";
        } else {
          return $"({this.Number} - {this.Subnumber}) {this.Name}";
        }
      }
    }


    [DataField("SUB_NUMERO_MAYOR")]
    public string Subnumber {
      get; private set;
    } = string.Empty;


    [DataField("PREFIJO_CUENTAS_AUXILIARES")]
    public string SubledgerAccountsPrefix {
      get; private set;
    } = string.Empty;


    [DataField("ID_MONEDA_BASE", ConvertFrom = typeof(long))]
    public Currency BaseCurrency {
      get; private set;
    }


    [DataField("ID_EMPRESA", ConvertFrom = typeof(long))]
    internal Organization Organization {
      get; private set;
    }


    [DataField("ID_CALENDARIO", ConvertFrom = typeof(long))]
    internal int CalendarId {
      get; private set;
    }


    [DataField("FECHA_CIERRE", Default = "ExecutionServer.DateMaxValue")]
    public DateTime EndDate {
      get; private set;
    }


    [DataField("ELIMINADO", ConvertFrom = typeof(int))]
    public bool Deleted {
      get; private set;
    }

    #endregion Public properties

    #region Public methods

    public LedgerAccount AssignAccount(StandardAccount standardAccount) {
      Assertion.AssertObject(standardAccount, "standardAccount");

      Assertion.Assert(standardAccount.AccountsChart.Equals(this.AccountsChart),
          $"La cuenta estandar con id {standardAccount.Id} " +
          $"pertenece al catálogo de cuentas de cuentas {standardAccount.AccountsChart.Name}, " +
          $"el cual no corresponde al catálogo asignado a la contabilidad {this.FullName}.");

      Assertion.Assert(standardAccount.Role != AccountRole.Sumaria,
             $"La cuenta estandar con id {standardAccount.Id} es sumaria. " +
             $"No se puede agregar a la lista de cuentas de la contabilidad {this.FullName}.");

      if (Contains(standardAccount)) {
        return GetAccount(standardAccount);
      }

      return LedgerData.AssignStandardAccount(this, standardAccount);
    }


    public bool Contains(StandardAccount standardAccount) {
      return LedgerData.ContainsLedgerAccount(this, standardAccount);
    }


    public SubledgerAccount CreateSubledgerAccount(string subledgerAccountNo, SubledgerType subledgerType) {
      subledgerAccountNo = FormatSubledgerAccount(subledgerAccountNo);

      SubledgerAccount subledgerAccount = this.TryGetSubledgerAccount(subledgerAccountNo);

      if (subledgerAccount != null) {
        return subledgerAccount;
      }

      Subledger subLedger = this.Subledgers().Find(x => x.SubledgerType.Equals(subledgerType));

      if (subLedger == null) {
        throw Assertion.AssertNoReachThisCode(
            $"No se ha definido una lista de auxiliares pendientes para la contabilidad {this.FullName}.");
      }

      return subLedger.CreateAccount(subledgerAccountNo);
    }


    public SubledgerAccount CreateSubledgerAccount(string subledgerAccountNo, SubledgerType subledgerType, string sublegderAccountName) {
      subledgerAccountNo = FormatSubledgerAccount(subledgerAccountNo);

      SubledgerAccount subledgerAccount = this.TryGetSubledgerAccount(subledgerAccountNo);

      if (subledgerAccount != null) {
        return subledgerAccount;
      }

      Subledger subLedger = this.Subledgers().Find(x => x.SubledgerType.Equals(subledgerType));

      if (subLedger == null) {
        throw Assertion.AssertNoReachThisCode(
            $"No se ha definido una lista de auxiliares pendientes para la contabilidad {this.FullName}.");
      }

      return subLedger.CreateAccount(subledgerAccountNo, sublegderAccountName);
    }



    public string FormatSubledgerAccount(string subledgerAccountNo) {
      const int SUBLEDGER_ACCOUNT_FIXED_LENGTH = 16;

      var temp = EmpiriaString.TrimAll(subledgerAccountNo);

      temp = temp.TrimStart('0');

      if (temp.Length == 0) {
        return string.Empty;
      }

      int prefixLength = this.SubledgerAccountsPrefix.Length;
      int accountPartLength = SUBLEDGER_ACCOUNT_FIXED_LENGTH;

      if (temp.Length == prefixLength + accountPartLength &&
          temp.StartsWith(this.SubledgerAccountsPrefix)) {
        return temp;

      } else if (temp.Length == prefixLength + accountPartLength &&
                 !temp.StartsWith(this.SubledgerAccountsPrefix)) {
        Assertion.AssertFail($"El auxiliar '{subledgerAccountNo}' tiene un formato que no " +
                             $"es válido para la contabilidad {this.FullName}.");

      } else if (temp.Length < accountPartLength) {
        return this.SubledgerAccountsPrefix + temp.PadLeft(SUBLEDGER_ACCOUNT_FIXED_LENGTH, '0');

      } else if (temp.Length > prefixLength + accountPartLength) {
        Assertion.AssertNoReachThisCode($"El auxiliar '{subledgerAccountNo}'" +
                                        $"tiene una longitud que excede el formato determinado.");
      }

      return temp;
    }


    public LedgerAccount GetAccount(StandardAccount standardAccount) {
      LedgerAccount account = LedgerData.TryGetLedgerAccount(this, standardAccount);

      Assertion.AssertObject(account, "account");

      return account;
    }


    public LedgerAccount TryGetAccount(StandardAccount standardAccount) {
      return LedgerData.TryGetLedgerAccount(this, standardAccount);
    }


    public SubledgerAccount TryGetSubledgerAccount(string subledgerAccountNo) {
      string formattedAccountNo = FormatSubledgerAccount(subledgerAccountNo);

      return LedgerData.TryGetSubledgerAccount(this, formattedAccountNo);
    }


    public LedgerAccount GetAccountWithId(int ledgerAccountId) {
      var ledgerAccount = LedgerAccount.Parse(ledgerAccountId);

      Assertion.Assert(ledgerAccount.Ledger.Equals(this),
          $"The ledger account with id {ledgerAccountId} does not belong to ledger '{this.Name}'.");

      return ledgerAccount;
    }


    public bool IsAccountingDateOpened(DateTime accountingDate) {
      FixedList<DateTime> openedAccoutingDates = this.OpenedAccountingDates();

      return openedAccoutingDates.Contains(accountingDate);
    }


    public NamedEntityDto MapToNamedEntity() {
      return new NamedEntityDto(this.UID, this.FullName);
    }


    public FixedList<DateTime> OpenedAccountingDates() {
      var calendar = Calendar.Parse(this.CalendarId);

      return calendar.OpenedAccountingDates();
    }


    public FixedList<Account> SearchUnassignedAccounts(string keywords, DateTime date) {
      return LedgerData.SearchUnassignedAccountsForEdition(this, keywords, date);
    }

    public FixedList<SubledgerType> SubledgerTypes() {
      return new FixedList<SubledgerType>(this.Subledgers().Select(x => x.SubledgerType));
    }

    public FixedList<Subledger> Subledgers() {
      return LedgerData.GetSubledgers(this);
    }

    #endregion Public methods

  }  // class Ledger

}  // namespace Empiria.FinancialAccounting
