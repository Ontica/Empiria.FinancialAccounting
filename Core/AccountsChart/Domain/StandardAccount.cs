/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Accounts Chart                             Component : Domain Layer                            *
*  Assembly : FinancialAccounting.Core.dll               Pattern   : Empiria Data Object                     *
*  Type     : StandardAccount                            License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Contains information about an standard account.                                                *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting {

  /// <summary>Contains information about an standard account.</summary>
  public class StandardAccount : BaseObject {

    #region Constructors and parsers


    protected StandardAccount() {
      // Required by Empiria Framework.
    }


    static public StandardAccount Parse(int id) {
      return BaseObject.ParseId<StandardAccount>(id);
    }


    static public StandardAccount Parse(string uid) {
      return BaseObject.ParseKey<StandardAccount>(uid);
    }


    static public StandardAccount Empty => BaseObject.ParseEmpty<StandardAccount>();

    #endregion Constructors and parsers

    #region Public properties


    [DataField("ID_TIPO_CUENTAS_STD", ConvertFrom = typeof(long))]
    public AccountsChart AccountsChart {
      get; private set;
    }


    [DataField("NUMERO_CUENTA_ESTANDAR")]
    public string Number {
      get; private set;
    } = string.Empty;


    [DataField("NOMBRE_CUENTA_ESTANDAR")]
    public string Name {
      get; private set;
    } = string.Empty;


    public string FullName {
      get {
        if (HasParent) {
          return $"{Name} - {GetParent().FullName}";
        } else if (!IsEmptyInstance) {
          return Name;
        } else {
          return string.Empty;
        }
      }
    }


    [DataField("DESCRIPCION")]
    public string Description {
      get; private set;
    } = string.Empty;


    [DataField("ROL_CUENTA", Default = AccountRole.Sumaria)]
    public AccountRole Role {
      get; private set;
    } = AccountRole.Sumaria;



    [DataField("ID_TIPO_CUENTA")]
    public AccountType AccountType {
      get;
      private set;
    }


    [DataField("NATURALEZA", Default = DebtorCreditorType.Deudora)]
    public DebtorCreditorType DebtorCreditor {
      get; private set;
    } = DebtorCreditorType.Deudora;


    public bool HasParent {
      get {
        return (this.Level > 1);
      }
    }


    public bool NotHasParent {
      get {
        return !this.HasParent;
      }
    }


    public int Level {
      get {
        if (this.IsEmptyInstance) {
          return 0;
        }
        var accountNumberSeparator = this.AccountsChart.MasterData.AccountNumberSeparator;

        return EmpiriaString.CountOccurences(Number, accountNumberSeparator) + 1;
      }
    }


    public string FirstLevelAccountNumber {
      get {
        if (this.AccountsChart.Equals(AccountsChart.IFRS)) {
          return this.Number.Substring(0, 1);
        } else {
          return this.Number.Substring(0, 4);
        }
      }
    }


    public string GroupNumber {
      get {
        if (this.AccountsChart.Equals(AccountsChart.IFRS)) {
          return this.Number.Substring(0, 1);
        } else {
          return this.Number.Substring(0, 2) + "00";
        }
      }
    }

    #endregion Public properties

    #region Public methods

    public Account GetHistoric(DateTime date) {
      return this.AccountsChart.GetAccountHistory(this.Number, date);
    }


    private StandardAccount _parent;
    public StandardAccount GetParent() {
      if (_parent != null) {
        return _parent;
      }
      if (!this.HasParent) {
        _parent = StandardAccount.Empty;
        return StandardAccount.Empty;
      }

      var accountNumberSeparator = this.AccountsChart.MasterData.AccountNumberSeparator;

      var parentAccountNumber = this.Number.Substring(0, this.Number.LastIndexOf(accountNumberSeparator));

      _parent = this.AccountsChart.GetStandardAccount(parentAccountNumber);

      return _parent;
    }


    #endregion Public methods

  }  // class StandardAccount

}  // namespace Empiria.FinancialAccounting
