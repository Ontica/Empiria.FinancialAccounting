/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Concepts                              Component : Interface adapters                 *
*  Assembly : FinancialAccounting.FinancialConcepts.dll       Pattern   : Enumerated type with extensions    *
*  Type     : EditFinancialConceptEntryCommandTypeExtensions  License   : Please read LICENSE.txt file       *
*                                                                                                            *
*  Summary  : Enumerates the command types used to create or update financial concept integration entries.   *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

namespace Empiria.FinancialAccounting.FinancialConcepts.Adapters {

  /// <summary>Enumerates the command types used to create or update
  /// financial concept integration entries.</summary>
  public enum EditFinancialConceptEntryCommandType {

    InsertAccountRule,

    InsertConceptReferenceRule,

    InsertExternalValueRule,

    UpdateAccountRule,

    UpdateConceptReferenceRule,

    UpdateExternalValueRule,

    Undefined

  }  // enum EditFinancialConceptEntryCommandType



  /// <summary>Extension methods for EditFinancialConceptEntryCommandType enumeration.</summary>
  static internal class EditFinancialConceptEntryCommandTypeExtensions {


    static internal bool ForInsert(this EditFinancialConceptEntryCommandType commandType) {
      return (commandType == EditFinancialConceptEntryCommandType.InsertAccountRule ||
              commandType == EditFinancialConceptEntryCommandType.InsertConceptReferenceRule ||
              commandType == EditFinancialConceptEntryCommandType.InsertExternalValueRule);
    }


    static internal bool ForUpdate(this EditFinancialConceptEntryCommandType commandType) {
      return (commandType == EditFinancialConceptEntryCommandType.UpdateAccountRule ||
              commandType == EditFinancialConceptEntryCommandType.UpdateConceptReferenceRule ||
              commandType == EditFinancialConceptEntryCommandType.UpdateExternalValueRule);
    }


    static internal bool OverAccount(this EditFinancialConceptEntryCommandType commandType) {
      return (commandType == EditFinancialConceptEntryCommandType.InsertAccountRule ||
              commandType == EditFinancialConceptEntryCommandType.UpdateAccountRule);
    }


    static internal bool OverConceptReference(this EditFinancialConceptEntryCommandType commandType) {
      return (commandType == EditFinancialConceptEntryCommandType.InsertConceptReferenceRule ||
              commandType == EditFinancialConceptEntryCommandType.UpdateConceptReferenceRule);
    }


    static internal bool OverExternalVariable(this EditFinancialConceptEntryCommandType commandType) {
      return (commandType == EditFinancialConceptEntryCommandType.InsertExternalValueRule ||
              commandType == EditFinancialConceptEntryCommandType.UpdateExternalValueRule);
    }


  }  // static class EditFinancialConceptEntryCommandTypeExtensions


}  // namespace Empiria.FinancialAccounting.FinancialConcepts.Adapters
