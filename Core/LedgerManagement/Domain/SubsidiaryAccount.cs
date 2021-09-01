/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Accounts Chart                             Component : Domain Layer                            *
*  Assembly : FinancialAccounting.Core.dll               Pattern   : Empiria Data Object                     *
*  Type     : SubsidiaryAccount                          License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Holds information about a subsidiary ledger account.                                           *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.FinancialAccounting.Data;

namespace Empiria.FinancialAccounting {

  /// <summary>Holds information about a subsidiary ledger account.</summary>
  public class SubsidiaryAccount : BaseObject {

    #region Constructors and parsers

    private SubsidiaryAccount() {
      // Required by Empiria Framework.
    }


    static public SubsidiaryAccount Parse(int id) {
      if (id == 0) {
        id = -1;
      }
      return BaseObject.ParseId<SubsidiaryAccount>(id);
    }

    static public FixedList<SubsidiaryAccount> GetList(AccountsChart accountsChart, string keywords) {
      Assertion.AssertObject(accountsChart, "accountsChart");
      Assertion.AssertObject(keywords, "keywords");

      return SubsidiaryLedgerData.GetSubsidiaryAccountsList(accountsChart, keywords);
    }

    static public void Preload() {
      BaseObject.GetList<SubsidiaryAccount>();
    }

    static public SubsidiaryAccount Empty => BaseObject.ParseEmpty<SubsidiaryAccount>();


    #endregion Constructors and parsers

    #region Public properties

    [DataField("ID_MAYOR_AUXILIAR", ConvertFrom = typeof(long))]
    public SubsidiaryLedger SubsidaryLedger {
      get; private set;
    }


    [DataField("NUMERO_CUENTA_AUXILIAR")]
    public string Number {
      get; private set;
    }


    [DataField("NOMBRE_CUENTA_AUXILIAR")]
    public string Name {
      get; private set;
    }


    [DataField("DESCRIPCION")]
    public string Description {
      get; private set;
    }


    [DataField("ELIMINADA", ConvertFrom = typeof(int))]
    public bool Deleted {
      get; private set;
    }

    #endregion Public properties

  }  // class SubsidiaryAccount

}  // namespace Empiria.FinancialAccounting
