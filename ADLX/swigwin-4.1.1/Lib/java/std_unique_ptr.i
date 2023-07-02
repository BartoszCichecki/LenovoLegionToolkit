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

%typemap (jni) std::unique_ptr< TYPE > "jlong"
%typemap (jtype) std::unique_ptr< TYPE > "long"
%typemap (jstype) std::unique_ptr< TYPE > "$typemap(jstype, TYPE)"

%typemap(in) std::unique_ptr< TYPE > (TYPE *unique_temp)
%{ unique_temp = *(TYPE **)&$input;
  $1.reset(unique_temp); %}

%typemap(javain) std::unique_ptr< TYPE > "$typemap(jstype, TYPE).swigRelease($javainput)"

%typemap (out) std::unique_ptr< TYPE > %{
  jlong lpp = 0;
  *(TYPE **) &lpp = $1.release();
  $result = lpp;
%}

%typemap(javaout) std::unique_ptr< TYPE > {
    long cPtr = $jnicall;
    return (cPtr == 0) ? null : new $typemap(jstype, TYPE)(cPtr, true);
  }

%typemap(typecheck, precedence=SWIG_TYPECHECK_POINTER, equivalent="TYPE *") std::unique_ptr< TYPE > ""

%template() std::unique_ptr< TYPE >;
%enddef

namespace std {
  template <class T> class unique_ptr {};
}
