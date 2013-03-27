using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Shared.Utilities.Enums;

namespace ThreadSafetyAnnotations.Engine
{
    public enum ErrorCode
    {
        [Description("Guarded field cannot use the same lock more than once.")]
        GUARDED_FIELD_USES_SAME_LOCK_MORE_THAN_ONCE,

        [Description("Lock hierachy conflicts with another Guarded Fields lock hiearchy.")]
        GUARDED_FIELD_LOCK_HIERARCHY_DECLARATION_CONFLICT,

        [Description("Guarded field was accessed outside of a lock statement.")]
        GUARDED_FIELD_ACCESSED_OUTSIDE_OF_LOCK,

        [Description("Declared field references unknown lock.")]
        GUARDED_FIELD_REFERENCES_UNKNOWN_LOCK,

        [Description("Lock is not guarding any field.")]
        LOCK_PROTECTS_NOTHING,

        [Description("Guarded field must be declared private.")]
        GUARDED_FIELD_IS_NOT_PRIVATE,

        [Description("Guarded field must be protected by at least one lock.")]
        GUARDED_FIELD_NOT_ASSOCIATED_WITH_A_LOCK,

        [Description("Lock must be declared private.")]
        LOCK_IS_NOT_PRIVATE,

        [Description("Lock must be of type System.Object")]
        LOCK_MUST_BE_SYSTEM_OBJECT,

        [Description("Class is not marked with the ThreadSafeAttribute but contains a guarded member.")]
        GUARDED_FIELD_IN_A_NON_THREAD_SAFE_CLASS,

        [Description("Class is not marked with the ThreadSafeAttribute but contains a lock.")]
        LOCK_IN_A_NON_THREAD_SAFE_CLASS,

        [Description("Class is marked with ThreadSafe attribute but contains no Locks or Guarded fields.")]
        CLASS_MUST_HAVE_LOCKS_OR_GUARDED_FIELDS,

        [Description("Classes marked with the ThreadSafe attribute cannot be partial.")]
        CLASS_CANNOT_BE_PARTIAL,

        [Description("Classes marked with the ThreadSafe attribute cannot be static.")]
        CLASS_CANNOT_BE_STATIC,

        [Description("Classes marked with the ThreadSafe attribute cannot be abstract.")]
        CLASS_CANNOT_BE_ABSTRACT,
    }
}
