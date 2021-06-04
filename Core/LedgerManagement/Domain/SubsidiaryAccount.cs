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

namespace Empiria.FinancialAccounting {

  /// <summary>Holds information about a subsidiary ledger account.</summary>
  public class SubsidiaryAccount : BaseObject {

    #region Constructors and parsers

    private SubsidiaryAccount() {
      // Required by Empiria Framework.
    }


    static public SubsidiaryAccount Parse(int id) {
      return BaseObject.ParseId<SubsidiaryAccount>(id);
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
