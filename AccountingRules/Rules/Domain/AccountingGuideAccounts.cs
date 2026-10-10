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

    public string CreditNoteVatAccount {
      get {
        return _extData.Get("creditNoteVatAccount", string.Empty);
      }
    }


    public string CreditNoteVatSubledgerAccount {
      get {
        return _extData.Get("creditNoteVatSubledgerAccount", string.Empty);
      }
    }


    public string PenaltyIncomeAccount {
      get {
        return _extData.Get("penaltyIncomeAccount", string.Empty);
      }
    }


    public string VatCreditableAccount {
      get {
        return _extData.Get("vatCreditableAccount", string.Empty);
      }
    }


    public string VatPendingAccount {
      get {
        return _extData.Get("vatPendingAccount", string.Empty);
      }
    }


    public string VatSubledgerAccount {
      get {
        return _extData.Get("vatSubledgerAccount", string.Empty);
      }
    }

    #endregion Properties

  }  // class AccountingGuideAccounts

}  // namespace Empiria.FinancialAccounting.AccountingRules
