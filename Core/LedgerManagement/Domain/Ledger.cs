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

namespace Empiria.FinancialAccounting {

  /// <summary>Holds information about an accounting ledger book.</summary>
  public class Ledger : BaseObject, INamedEntity {

    #region Constructors and parsers

    private Ledger() {
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
    public string SubsidiaryAccountsPrefix {
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

    public LedgerAccount GetAccountWithId(int ledgerAccountId) {
      var ledgerAccount = LedgerAccount.Parse(ledgerAccountId);

      Assertion.Assert(ledgerAccount.Ledger.Equals(this),
          $"The ledger account with id {ledgerAccountId} does not belong to ledger '{this.Name}'.");

      return ledgerAccount;
    }


    public NamedEntityDto MapToNamedEntity() {
      return new NamedEntityDto(this.UID, this.FullName);
    }


    public FixedList<DateTime> OpenedAccountingDates() {
      var calendar = Calendar.Parse(this.CalendarId);

      return calendar.OpenedAccountingDates();
    }


    #endregion Public methods

  }  // class Ledger

}  // namespace Empiria.FinancialAccounting
