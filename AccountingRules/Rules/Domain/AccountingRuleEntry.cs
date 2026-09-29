/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Accounting Rules                   Component : Domain Layer                          *
*  Assembly : FinancialAccounting.AccountingRules.dll      Pattern   : Information holder                    *
*  Type     : AccountingRuleEntry                          License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Contains information about an accounting rule entry related to an accounting rule.             *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using Empiria.Json;
using Empiria.Parties;
using Empiria.StateEnums;

using Empiria.FinancialAccounting.Vouchers;

namespace Empiria.FinancialAccounting.AccountingRules {

  /// <summary>Contains information about an accounting rule entry related to an accounting rule.</summary>
  public sealed class AccountingRuleEntry : BaseObject {

    #region Constructors and parsers

    private AccountingRuleEntry() {
      // no-op  
    }

    static public AccountingRuleEntry Parse(int id) => ParseId<AccountingRuleEntry>(id);

    static public AccountingRuleEntry Parse(string uid) => ParseKey<AccountingRuleEntry>(uid);

    static public AccountingRuleEntry Empty => ParseEmpty<AccountingRuleEntry>();

    #endregion Constructors and parsers

    #region Properties

    [DataField("ACR_ENTRY_RULE_ID")]
    public AccountingRule Rule {
      get; private set;
    }


    [DataField("ACR_ENTRY_ROLE_CODE")]
    public string RoleCode {
      get; private set;
    }


    [DataField("ACR_ENTRY_IS_REQUIRED", Default = true, ConvertFrom = typeof(int))]
    public bool IsRequired {
      get; private set;
    }


    [DataField("ACR_ENTRY_ENTRY_TYPE", Default = VoucherEntryType.Debit)]
    public VoucherEntryType EntryType {
      get; private set;
    }


    [DataField("ACR_ENTRY_STD_ACCOUNT_ID")]
    public StandardAccount StandardAccount {
      get; private set;
    }


    [DataField("ACR_ENTRY_AMOUNT_SOURCE")]
    public string AmountSource {
      get; private set;
    }


    [DataField("ACR_ENTRY_SUBLEDGER_SOURCE")]
    public string SubledgerAccountSource {
      get; private set;
    }


    [DataField("ACR_ENTRY_SELECTOR_SOURCE")]
    public string SelectorValueSource {
      get; private set;
    }


    [DataField("ACR_ENTRY_ACCT_CLASS_SOURCE")]
    public string AccountingClassificationSource {
      get; private set;
    }


    [DataField("ACR_ENTRY_CONTROL_NO_SOURCE")]
    public string ControlNoSource {
      get; private set;
    }


    [DataField("ACR_ENTRY_RESP_AREA_SOURCE")]
    public string ResponsibilityAreaSource {
      get; private set;
    }


    [DataField("ACR_ENTRY_EXT_DATA")]
    internal JsonObject ExtData {
      get; private set;
    }


    [DataField("ACR_ENTRY_POSITION")]
    public int Position {
      get; private set;
    }


    [DataField("ACR_ENTRY_POSTED_BY_ID")]
    public Party PostedBy {
      get; private set;
    }


    [DataField("ACR_ENTRY_STATUS", Default = EntityStatus.Active)]
    public EntityStatus Status {
      get; private set;
    }


    public string Keywords {
      get {
        return EmpiriaString.BuildKeywords(StandardAccount.FullName, RoleCode);
      }
    }

    #endregion Properties

  }  // class AccountingRuleEntry

}  // namespace Empiria.FinancialAccounting.AccountingRules
