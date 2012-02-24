using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ThreadSafetyAnnotations.Engine
{
    public enum ErrorCode
    {
        GUARDED_MEMBER_REFERENCES_UNKNOWN_LOCK,
        LOCK_PROTECTS_NOTHING,
        GUARDED_MEMBER_IS_NOT_PRIVATE,
        GUARDED_MEMBER_NOT_ASSOCIATED_WITH_A_LOCK,
        LOCK_IS_NOT_PRIVATE,
        LOCK_MUST_BE_SYSTEM_OBJECT,
        GUARDED_MEMBER_IN_A_NON_THREAD_SAFE_CLASS,
        LOCK_IN_A_NON_THREAD_SAFE_CLASS,
        CLASS_CANNOT_BE_PARTIAL,
        CLASS_CANNOT_BE_STATIC,
        CLASS_CANNOT_BE_ABSTRACT,
    }
}
