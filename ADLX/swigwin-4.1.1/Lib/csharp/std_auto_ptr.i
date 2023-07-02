/* -----------------------------------------------------------------------------
 * std_auto_ptr.i
 *
 * SWIG library file for handling std::auto_ptr.
 * Memory ownership is passed from the std::auto_ptr C++ layer to the proxy
 * class when returning a std::auto_ptr from a function.
 * Memory ownership is passed from the proxy class to the std::auto_ptr in the
 * C++ layer when passed as a parameter to a wrapped function.
 * ----------------------------------------------------------------------------- */

%define %auto_ptr(TYPE)
%typemap (ctype) std::auto_ptr< TYPE > "void *"
%typemap (imtype, out="System.IntPtr") std::auto_ptr< TYPE > "global::System.Runtime.InteropServices.HandleRef"
%typemap (cstype) std::auto_ptr< TYPE > "$typemap(cstype, TYPE)"

%typemap(in) std::auto_ptr< TYPE >
%{ $1.reset((TYPE *)$input); %}

%typemap(csin) std::auto_ptr< TYPE > "$typemap(cstype, TYPE).swigRelease($csinput)"

%typemap (out) std::auto_ptr< TYPE > %{
  $result = (void *)$1.release();
%}

%typemap(csout, excode=SWIGEXCODE) std::auto_ptr< TYPE > {
    System.IntPtr cPtr = $imcall;
    $typemap(cstype, TYPE) ret = (cPtr == System.IntPtr.Zero) ? null : new $typemap(cstype, TYPE)(cPtr, true);$excode
    return ret;
  }

%typemap(typecheck, precedence=SWIG_TYPECHECK_POINTER, equivalent="TYPE *") std::auto_ptr< TYPE > ""

%template() std::auto_ptr< TYPE >;
%enddef

namespace std {
  template <class T> class auto_ptr {};
}
