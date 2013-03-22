using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Shared.Utilities.Enums;

namespace ThreadSafetyAnnotations.Engine
{
    public enum ErrorCode
    {
        [Description("Declared member references unknown lock.")]
        GUARDED_MEMBER_REFERENCES_UNKNOWN_LOCK,

        [Description("Lock is not guarding any member.")]
        LOCK_PROTECTS_NOTHING,

        [Description("Guarded member must be declared private.")]
        GUARDED_MEMBER_IS_NOT_PRIVATE,

        [Description("Guarded member must be protected by at least one lock.")]
        GUARDED_MEMBER_NOT_ASSOCIATED_WITH_A_LOCK,

        [Description("Lock must be declared private.")]
        LOCK_IS_NOT_PRIVATE,

        [Description("Lock must be of type System.Object")]
        LOCK_MUST_BE_SYSTEM_OBJECT,

        [Description("Class is not marked with the ThreadSafeAttribute but contains a guarded member.")]
        GUARDED_MEMBER_IN_A_NON_THREAD_SAFE_CLASS,

        [Description("Class is not marked with the ThreadSafeAttribute but contains a lock.")]
        LOCK_IN_A_NON_THREAD_SAFE_CLASS,

        [Description("Classes marked with the ThreadSafe attribute cannot be partial.")]
        CLASS_CANNOT_BE_PARTIAL,

        [Description("Classes marked with the ThreadSafe attribute cannot be static.")]
        CLASS_CANNOT_BE_STATIC,

        [Description("Classes marked with the ThreadSafe attribute cannot be abstract.")]
        CLASS_CANNOT_BE_ABSTRACT,
    }
}
