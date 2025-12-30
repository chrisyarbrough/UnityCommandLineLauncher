.PHONY: build install clean help

BINARY_NAME=unity
INSTALL_PATH=$(HOME)/bin

build:
	go build -o $(BINARY_NAME) .

install: build
	mkdir -p $(INSTALL_PATH)
	cp $(BINARY_NAME) $(INSTALL_PATH)/
	@echo "Installed to $(INSTALL_PATH)/$(BINARY_NAME)"
	@echo "Make sure $(INSTALL_PATH) is in your PATH"

clean:
	rm -f $(BINARY_NAME)

help:
	@echo "Available targets:"
	@echo "  build   - Build the binary"
	@echo "  install - Build and install to ~/bin"
	@echo "  clean   - Remove built binary"
	@echo "  help    - Show this help message"
