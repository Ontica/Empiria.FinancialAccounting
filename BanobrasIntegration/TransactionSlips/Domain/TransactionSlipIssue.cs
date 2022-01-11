/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Transaction Slips                             Component : Domain types                         *
*  Assembly : FinancialAccounting.BanobrasIntegration.dll   Pattern   : Information holder                   *
*  Type     : TransactionSlipIssue                          License   : Please read LICENSE.txt file         *
*                                                                                                            *
*  Summary  : Holds data about a transaction slip importation issue.                                         *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

namespace Empiria.FinancialAccounting.BanobrasIntegration.TransactionSlips {

  /// <summary>Holds data about a transaction slip importation issue.</summary>
  internal class TransactionSlipIssue {

    #region Properties

    [DataField("ID_VOLANTE_ISSUE")]
    public long Id {
      get;
      private set;
    }


    [DataField("ID_VOLANTE")]
    public long TransactionSlipId {
      get;
      private set;
    }


    [DataField("ID_VOLANTE_MOVIMIENTO")]
    public long TransactionSlipEntryId {
      get;
      private set;
    }


    [DataField("DESCRIPCION")]
    public string Description {
      get;
      private set;
    }


    #endregion Properties

  }  // class TransactionSlipIssue

}  // namespace Empiria.FinancialAccounting.BanobrasIntegration.TransactionSlips
