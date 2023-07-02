/* -----------------------------------------------------------------------------
 * director.swg
 *
 * This file contains support for director classes so that PHP proxy
 * methods can be called from C++.
 * ----------------------------------------------------------------------------- */

#ifndef SWIG_DIRECTOR_PHP_HEADER_
#define SWIG_DIRECTOR_PHP_HEADER_

#define SWIG_DIRECTOR_CAST(ARG) dynamic_cast<Swig::Director *>(ARG)

#include <string>
#include <exception>
#include <map>

namespace Swig {

  /* memory handler */
  struct GCItem {
    virtual ~GCItem() {
    }

    virtual int get_own() const {
      return 0;
    }
  };

  struct GCItem_var {
    GCItem_var(GCItem *item = 0) : _item(item) {
    }

    GCItem_var& operator=(GCItem *item) {
      GCItem *tmp = _item;
      _item = item;
      delete tmp;
      return *this;
    }

    ~GCItem_var() {
      delete _item;
    }

    GCItem * operator->() const {
      return _item;
    }

    private:
    GCItem *_item;
  };

  struct GCItem_Object : GCItem {
    GCItem_Object(int own) : _own(own) {
    }

    virtual ~GCItem_Object() {
    }

    int get_own() const {
      return _own;
    }

    private:
    int _own;
  };

  template <typename Type>
  struct GCItem_T : GCItem {
    GCItem_T(Type *ptr) : _ptr(ptr) {
    }

    virtual ~GCItem_T() {
      delete _ptr;
    }

    private:
    Type *_ptr;
  };

  class Director {
    private:
      /* flag indicating whether the object is owned by PHP or C++ */
      mutable bool swig_disown_flag;

    protected:
      // "mutable" so we can get a non-const pointer to it in const methods.
      mutable zval swig_self;
      typedef std::map<void *, GCItem_var> swig_ownership_map;
      mutable swig_ownership_map swig_owner;

    public:
      Director(zval *self) : swig_disown_flag(false) {
        ZVAL_COPY_VALUE(&swig_self, self);
      }

      ~Director() {
        if (swig_disown_flag) {
          Z_DELREF(swig_self);
	}
      }

      zend_object *swig_get_self() const { return Z_OBJ(swig_self); }

      void swig_disown() const {
        if (!swig_disown_flag) {
          swig_disown_flag = true;
          Z_ADDREF(swig_self);
        }
      }

      template <typename Type>
      void swig_acquire_ownership(Type *vptr) const {
        if (vptr) {
          swig_owner[vptr] = new GCItem_T<Type>(vptr);
        }
      }

      void swig_acquire_ownership_obj(void *vptr, int own) const {
        if (vptr && own) {
          swig_owner[vptr] = new GCItem_Object(own);
        }
      }
  };

  /* base class for director exceptions */
  class DirectorException : public std::exception {
  protected:
    std::string swig_msg;
  public:
    DirectorException(int code, const char *hdr, const char *msg) : swig_msg(hdr) {
      if (msg && msg[0]) {
        swig_msg += " ";
        swig_msg += msg;
      }
      // Don't replace an already active PHP exception.
      if (!EG(exception)) zend_throw_exception(NULL, swig_msg.c_str(), code);
    }

    virtual ~DirectorException() throw() {
    }

    const char *what() const throw() {
      return swig_msg.c_str();
    }

    static void raise(int code, const char *hdr, const char *msg) {
      throw DirectorException(code, hdr, msg);
    }
  };

  /* attempt to call a pure virtual method via a director method */
  class DirectorPureVirtualException : public DirectorException {
  public:
    DirectorPureVirtualException(const char *msg)
      : DirectorException(E_ERROR, "SWIG director pure virtual method called", msg) {
    }

    static void raise(const char *msg) {
      throw DirectorPureVirtualException(msg);
    }
  };

  /* any php exception that occurs during a director method call */
  class DirectorMethodException : public DirectorException {
  public:
    DirectorMethodException()
      : DirectorException(E_ERROR, "SWIG director method error", NULL) {
    }

    DirectorMethodException(const char *msg)
      : DirectorException(E_ERROR, "SWIG director method error", msg) {
    }

    static void raise(const char *msg) {
      throw DirectorMethodException(msg);
    }
  };
}

#endif
