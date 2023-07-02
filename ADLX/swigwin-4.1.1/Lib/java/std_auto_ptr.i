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

%typemap (jni) std::auto_ptr< TYPE > "jlong"
%typemap (jtype) std::auto_ptr< TYPE > "long"
%typemap (jstype) std::auto_ptr< TYPE > "$typemap(jstype, TYPE)"

%typemap(in) std::auto_ptr< TYPE > (TYPE *auto_temp)
%{ auto_temp = *(TYPE **)&$input;
  $1.reset(auto_temp); %}

%typemap(javain) std::auto_ptr< TYPE > "$typemap(jstype, TYPE).swigRelease($javainput)"

%typemap (out) std::auto_ptr< TYPE > %{
  jlong lpp = 0;
  *(TYPE **) &lpp = $1.release();
  $result = lpp;
%}

%typemap(javaout) std::auto_ptr< TYPE > {
    long cPtr = $jnicall;
    return (cPtr == 0) ? null : new $typemap(jstype, TYPE)(cPtr, true);
  }

%typemap(typecheck, precedence=SWIG_TYPECHECK_POINTER, equivalent="TYPE *") std::auto_ptr< TYPE > ""

%template() std::auto_ptr< TYPE >;
%enddef

namespace std {
  template <class T> class auto_ptr {};
}
