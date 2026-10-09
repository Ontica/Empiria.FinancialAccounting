/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Accounting Rules                   Component : Domain Layer                          *
*  Assembly : FinancialAccounting.AccountingRules.dll      Pattern   : Aggregate root                        *
*  Type     : AccountingGuideAccounts                      License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Holds accounts information for an accounting guide.                                            *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using Empiria.Json;

namespace Empiria.FinancialAccounting.AccountingRules {

  /// <summary>Holds accounts information for an accounting guide.</summary>
  public class AccountingGuideAccounts {

    #region Fields

    private readonly JsonObject _extData;

    #endregion Fields

    #region Constructors and parsers

    internal AccountingGuideAccounts(JsonObject extData) {
      Assertion.Require(extData, nameof(extData));

      _extData = extData;
    }

    #endregion Constructors and parsers

    #region Properties

    public string PenaltyIncomeAccount {
      get {
        return _extData.Get<string>("penaltyIncomeAccount");
      }
    }


    public string PenaltyVatAccount {
      get {
        return _extData.Get<string>("penaltyVatAccount");
      }
    }


    public string VatCreditableAccount {
      get {
        return _extData.Get<string>("vatCreditableAccount");
      }
    }


    public string VatPendingAccount {
      get {
        return _extData.Get<string>("vatPendingAccount");
      }
    }

    #endregion Properties

  }  // class AccountingGuideAccounts

}  // namespace Empiria.FinancialAccounting.AccountingRules
