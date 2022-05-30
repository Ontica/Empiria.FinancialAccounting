/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Concepts                         Component : Interface adapters                      *
*  Assembly : FinancialAccounting.FinancialConcepts.dll  Pattern   : Data Transfer Object                    *
*  Type     : FinancialConceptEntryDto                   License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Data transfer object for FinancialConceptEntry instances.                                      *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.FinancialConcepts.Adapters {

  /// <summary>Data transfer object for FinancialConceptEntry instances.</summary>
  public class FinancialConceptEntryDto : IDto {

    internal FinancialConceptEntryDto() {
      // no-op
    }

    public string UID {
      get; internal set;
    }


    public FinancialConceptEntryType Type {
      get; internal set;
    }


    public string ItemName {
      get; internal set;
    }


    public string ItemCode {
      get; internal set;
    }


    public string SubledgerAccount {
      get; internal set;
    }


    public string SectorCode {
      get; internal set;
    }


    public string Operator {
      get; internal set;
    }

  }  // class FinancialConceptEntryDto

}  // namespace Empiria.FinancialAccounting.FinancialConcepts.Adapters
