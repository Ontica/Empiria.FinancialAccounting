/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Concepts                         Component : Interface adapters                      *
*  Assembly : FinancialAccounting.FinancialConcepts.dll  Pattern   : Data Transfer Object                    *
*  Type     : FinancialConceptIntegrationEntryDto        License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Data transfer object for FinancialConceptIntegrationEntry instances.                           *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.FinancialConcepts.Adapters {

  /// <summary>Data transfer object for FinancialConceptIntegrationEntry instances.</summary>
  public class FinancialConceptIntegrationEntryDto {

    internal FinancialConceptIntegrationEntryDto() {
      // no-op
    }

    public string UID {
      get; internal set;
    }


    public IntegrationEntryType Type {
      get;
      internal set;
    }

    public string ItemName {
      get;
      internal set;
    }

    public string ItemCode {
      get;
      internal set;
    }

    public string SubledgerAccount {
      get;
      internal set;
    }

    public string SectorCode {
      get;
      internal set;
    }

    public string Operator {
      get;
      internal set;
    }

  }  // class FinancialConceptIntegrationEntryDto

}  // namespace Empiria.FinancialAccounting.FinancialConcepts.Adapters
