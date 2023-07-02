/* -----------------------------------------------------------------------------
 * std_unique_ptr.i
 *
 * SWIG library file for handling std::unique_ptr.
 * Memory ownership is passed from the std::unique_ptr C++ layer to the proxy
 * class when returning a std::unique_ptr from a function.
 * Memory ownership is passed from the proxy class to the std::unique_ptr in the
 * C++ layer when passed as a parameter to a wrapped function.
 * ----------------------------------------------------------------------------- */

%define %unique_ptr(TYPE)
%typemap (ctype) std::unique_ptr< TYPE > "void *"
%typemap (imtype, out="System.IntPtr") std::unique_ptr< TYPE > "global::System.Runtime.InteropServices.HandleRef"
%typemap (cstype) std::unique_ptr< TYPE > "$typemap(cstype, TYPE)"

%typemap(in) std::unique_ptr< TYPE >
%{ $1.reset((TYPE *)$input); %}

%typemap(csin) std::unique_ptr< TYPE > "$typemap(cstype, TYPE).swigRelease($csinput)"

%typemap (out) std::unique_ptr< TYPE > %{
  $result = (void *)$1.release();
%}

%typemap(csout, excode=SWIGEXCODE) std::unique_ptr< TYPE > {
    System.IntPtr cPtr = $imcall;
    $typemap(cstype, TYPE) ret = (cPtr == System.IntPtr.Zero) ? null : new $typemap(cstype, TYPE)(cPtr, true);$excode
    return ret;
  }

%typemap(typecheck, precedence=SWIG_TYPECHECK_POINTER, equivalent="TYPE *") std::unique_ptr< TYPE > ""

%template() std::unique_ptr< TYPE >;
%enddef

namespace std {
  template <class T> class unique_ptr {};
}
